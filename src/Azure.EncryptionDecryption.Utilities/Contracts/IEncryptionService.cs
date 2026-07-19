using Azure.EncryptionDecryption.Utilities.Models;

namespace Azure.EncryptionDecryption.Utilities.Contracts
{
    public interface IEncryptionService
    {
        Task<EncryptionResponse> EncryptAsync(RequestParams requestParams);
    }
}