namespace Azure.EncryptionDecryption.Utilities.Models
{
    public class DecryptionResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string DecryptedBody { get; set; } = string.Empty;
    }
}