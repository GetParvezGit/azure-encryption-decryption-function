using Azure.EncryptionDecryption.Utilities.Models;

namespace Azure.EncryptionDecryption.Utilities.Contracts
{
    public interface IDecryptionService
    {
        Task<DecryptionResponse> DecryptAsync(RequestParams requestParams);
    }
}