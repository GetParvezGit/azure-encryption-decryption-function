using Azure.EncryptionDecryption.Utilities.Contracts;
using Azure.EncryptionDecryption.Utilities.Models;
using Azure.EncryptionDecryption.Utilities.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PgpCore;
using System.Text;

namespace Azure.EncryptionDecryption.Utilities.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EncryptionService> _logger;

        public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<EncryptionResponse> EncryptAsync(RequestParams requestParams)
        {
            if (requestParams == null || string.IsNullOrWhiteSpace(requestParams.Body))
            {
                return new EncryptionResponse
                {
                    IsSuccess = false,
                    Message = Constants.ResponseMessages.BodyRequired
                };
            }

            try
            {
                string publicKey = GetBase64DecodedPublicKey();

                string encryptedBody = await EncryptBodyUsingPgpAsync(requestParams.Body, publicKey);

                return new EncryptionResponse
                {
                    IsSuccess = true,
                    Message = Constants.ResponseMessages.EncryptionSuccess,
                    EncryptedBody = encryptedBody
                };
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, Constants.ResponseMessages.InvalidPublicKey);

                return new EncryptionResponse
                {
                    IsSuccess = false,
                    Message = Constants.ResponseMessages.InvalidPublicKey
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.ResponseMessages.EncryptionFailed);

                return new EncryptionResponse
                {
                    IsSuccess = false,
                    Message = $"{Constants.ResponseMessages.EncryptionFailed} {ex.Message}"
                };
            }
        }

        private string GetBase64DecodedPublicKey()
        {
            string? encodedPublicKey = _configuration[Constants.AppSettings.PgpPublicKey];

            if (string.IsNullOrWhiteSpace(encodedPublicKey))
            {
                throw new InvalidOperationException(Constants.ResponseMessages.PublicKeyMissing);
            }

            byte[] publicKeyBytes = Convert.FromBase64String(encodedPublicKey);

            string publicKey = Encoding.UTF8.GetString(publicKeyBytes);

            return publicKey;
        }

        private static async Task<string> EncryptBodyUsingPgpAsync(string plainText, string publicKey)
        {
            using Stream publicKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(publicKey));

            using Stream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(plainText));

            using MemoryStream encryptedStream = new();

            EncryptionKeys encryptionKeys = new(publicKeyStream);

            using PGP pgp = new(encryptionKeys);

            await pgp.EncryptStreamAsync(inputStream, encryptedStream);

            encryptedStream.Position = 0;

            return Convert.ToBase64String(encryptedStream.ToArray());
        }
    }
}