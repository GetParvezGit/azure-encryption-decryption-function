using Azure.EncryptionDecryption.Utilities.Contracts;
using Azure.EncryptionDecryption.Utilities.Models;
using Azure.EncryptionDecryption.Utilities.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PgpCore;
using System.Text;

namespace Azure.EncryptionDecryption.Utilities.Services
{
    public class DecryptionService : IDecryptionService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DecryptionService> _logger;

        public DecryptionService(IConfiguration configuration, ILogger<DecryptionService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<DecryptionResponse> DecryptAsync(RequestParams requestParams)
        {
            if (requestParams == null || string.IsNullOrWhiteSpace(requestParams.Body))
            {
                return new DecryptionResponse
                {
                    IsSuccess = false,
                    Message = Constants.ResponseMessages.BodyRequired
                };
            }

            try
            {
                string privateKey = GetBase64DecodedPrivateKey();
                string passPhrase = GetPassPhrase();

                string decryptedBody = await DecryptBodyUsingPgpAsync(requestParams.Body, privateKey, passPhrase);

                return new DecryptionResponse
                {
                    IsSuccess = true,
                    Message = Constants.ResponseMessages.DecryptionSuccess,
                    DecryptedBody = decryptedBody
                };
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, Constants.ResponseMessages.InvalidEncryptedBody);

                return new DecryptionResponse
                {
                    IsSuccess = false,
                    Message = Constants.ResponseMessages.InvalidEncryptedBody
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.ResponseMessages.DecryptionFailed);

                return new DecryptionResponse
                {
                    IsSuccess = false,
                    Message = $"{Constants.ResponseMessages.DecryptionFailed} {ex.Message}"
                };
            }
        }

        private string GetBase64DecodedPrivateKey()
        {
            string? encodedPrivateKey = _configuration[Constants.AppSettings.PgpPrivateKey];

            if (string.IsNullOrWhiteSpace(encodedPrivateKey))
            {
                throw new InvalidOperationException(
                    Constants.ResponseMessages.PrivateKeyMissing);
            }

            byte[] privateKeyBytes = Convert.FromBase64String(encodedPrivateKey);

            string privateKey = Encoding.UTF8.GetString(privateKeyBytes);

            return privateKey;
        }

        private string GetPassPhrase()
        {
            string? passPhrase = _configuration[Constants.AppSettings.PgpPassPhrase];

            if (string.IsNullOrWhiteSpace(passPhrase))
            {
                throw new InvalidOperationException(
                    Constants.ResponseMessages.PassPhraseMissing);
            }

            return passPhrase;
        }

        private static async Task<string> DecryptBodyUsingPgpAsync(string encryptedBase64Body, string privateKey, string passPhrase)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64Body);

            using Stream privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKey));

            using Stream encryptedInputStream = new MemoryStream(encryptedBytes);

            using MemoryStream decryptedOutputStream = new();

            EncryptionKeys encryptionKeys = new(privateKeyStream, passPhrase);

            using PGP pgp = new(encryptionKeys);

            await pgp.DecryptStreamAsync(encryptedInputStream, decryptedOutputStream);

            decryptedOutputStream.Position = 0;

            using StreamReader reader = new(decryptedOutputStream, Encoding.UTF8);

            return await reader.ReadToEndAsync();
        }
    }
}