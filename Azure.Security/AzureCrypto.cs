namespace Azure.Security
{
    using System;
    using System.Text;
    using System.Security.Cryptography;
    using System.IO;
    using Interfaces;

    public class AzureCrypto : ICrypto
    {
        private static ISymmetricKeyCache keyStore;

        public AzureCrypto(ISymmetricKeyCache store)
        {
            keyStore = store;
        }

        public string EncryptStringAndBase64(string s, Guid userId)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            var cryptedBytes = Encrypt(bytes, userId);
            return Convert.ToBase64String(cryptedBytes);
        }

        public string DecryptStringFromBase64(string base64String, Guid userId)
        {
            var bytes = Decrypt(Convert.FromBase64String(base64String), userId);
            return Encoding.Unicode.GetString(bytes);
        }

        public byte[] Encrypt(byte[] bytes, Guid userId)
        {
            using (var msEncrypted = new MemoryStream())
            {
                using (var encryptor = keyStore.GetEncryptor(userId))
                {
                    using (var csEncrypt = new CryptoStream(msEncrypted, encryptor, CryptoStreamMode.Write))
                    {
                        using (var inStream = new MemoryStream(bytes))
                        {
                            inStream.CopyTo(csEncrypt);
                        }
                        csEncrypt.Close();
                    }
                    return msEncrypted.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] bytes, Guid userId)
        {
            using (var decryptor = keyStore.GetDecryptor(userId))
            {
                using (var msDecrypted = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msDecrypted, decryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(bytes, 0, bytes.Length);
                    }
                    return msDecrypted.ToArray();
                }
            }
        }

        public ICryptoTransform GetEncryptor(Guid userId)
        {
            return keyStore.GetEncryptor(userId);
        }

        public ICryptoTransform GetDecryptor(Guid userId)
        {
            return keyStore.GetDecryptor(userId);
        }
    }
}
