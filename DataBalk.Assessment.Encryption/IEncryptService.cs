namespace DataBalk.Assessment.Encryption
{
    public interface IEncryptService
    {
        EncryptHashSalt Encrypt(string textToEncrypt);

        bool VerifyEncryption(string textToVerify, byte[] salt, string encryptedText);
    }
}
