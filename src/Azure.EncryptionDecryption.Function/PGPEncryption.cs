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
    public class PGPEncryption
    {
        private readonly ILogger<PGPEncryption> _logger;
        private readonly IEncryptionService _encryptionService;

        public PGPEncryption(ILogger<PGPEncryption> logger, IEncryptionService encryptionService)
        {
            _logger = logger;
            _encryptionService = encryptionService;
        }

        [Function(Constants.FunctionNames.PGPEncryption)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("PGP encryption HTTP trigger function started.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new EncryptionResponse
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

            EncryptionResponse response = await _encryptionService.EncryptAsync(requestParams!);

            if (!response.IsSuccess)
            {
                return new BadRequestObjectResult(response);
            }

            _logger.LogInformation("PGP encryption HTTP trigger function completed.");

            return new OkObjectResult(response);
        }
    }
}