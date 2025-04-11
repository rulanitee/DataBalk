using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace DataBalk.Assessment.Utils
{
    public static class EncryptionExtension
    {
        public static EncryptHashSalt Encrypt(string textToEncrypt)
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = RandomNumberGenerator.Create())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            string hash = Hash(textToEncrypt, salt);
            return new EncryptHashSalt() { Hash = hash, Salt = salt };
        }

        public static bool VerifyEncryption(string textToVerify, byte[] salt, string encryptedText)
        {
            string hash = Hash(textToVerify, salt);
            return encryptedText.Equals(hash);
        }

        private static string Hash(string textToEncrypt, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: textToEncrypt,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
        }
    }
}
