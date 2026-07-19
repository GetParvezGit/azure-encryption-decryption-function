namespace Azure.EncryptionDecryption.Utilities.Utils
{
    public static class Constants
    {
        public static class AppSettings
        {
            public const string PgpPublicKey = "PGP_PUBLIC_KEY";
            public const string PgpPrivateKey = "PGP_PRIVATE_KEY";
            public const string PgpPassPhrase = "PGP_PASSPHRASE";
        }

        public static class ResponseMessages
        {
            public const string EncryptionSuccess = "Body encrypted successfully.";
            public const string DecryptionSuccess = "Body decrypted successfully.";

            public const string RequestBodyRequired = "Request body is required.";
            public const string BodyRequired = "Body value is required.";

            public const string PublicKeyMissing = "PGP public key is missing in configuration.";
            public const string PrivateKeyMissing = "PGP private key is missing in configuration.";
            public const string PassPhraseMissing = "PGP passphrase is missing in configuration.";

            public const string InvalidPublicKey = "PGP public key is not a valid Base64 encoded value.";
            public const string InvalidPrivateKey = "PGP private key is not a valid Base64 encoded value.";
            public const string InvalidEncryptedBody = "Encrypted body is not a valid Base64 encoded value.";

            public const string EncryptionFailed = "PGP encryption failed.";
            public const string DecryptionFailed = "PGP decryption failed.";
        }

        public static class FunctionNames
        {
            public const string PGPEncryption = "PGPEncryption";
            public const string PGPDecryption = "PGPDecryption";
        }
    }
}