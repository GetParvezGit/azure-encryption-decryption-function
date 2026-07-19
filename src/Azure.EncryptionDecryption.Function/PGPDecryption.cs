using Azure.EncryptionDecryption.Utilities.Contracts;
using Azure.EncryptionDecryption.Utilities.Models;
using Azure.EncryptionDecryption.Utilities.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Azure.EncryptionDecryption.Function
{
    public class PGPDecryption
    {
        private readonly ILogger<PGPDecryption> _logger;
        private readonly IDecryptionService _decryptionService;

        public PGPDecryption(ILogger<PGPDecryption> logger, IDecryptionService decryptionService)
        {
            _logger = logger;
            _decryptionService = decryptionService;
        }

        [Function(Constants.FunctionNames.PGPDecryption)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("PGP decryption HTTP trigger function started.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new DecryptionResponse
                {
                    IsSuccess = false,
                    Message = Constants.ResponseMessages.RequestBodyRequired
                });
            }

            RequestParams? requestParams = JsonSerializer.Deserialize<RequestParams>(
                requestBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            DecryptionResponse response =
                await _decryptionService.DecryptAsync(requestParams!);

            if (!response.IsSuccess)
            {
                return new BadRequestObjectResult(response);
            }

            _logger.LogInformation("PGP decryption HTTP trigger function completed.");

            return new OkObjectResult(response);
        }
    }
}