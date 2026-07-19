namespace Azure.EncryptionDecryption.Utilities.Models
{
    public class EncryptionResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string EncryptedBody { get; set; } = string.Empty;
    }
}