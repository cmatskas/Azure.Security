namespace Azure.Security
{
    using Interfaces;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class AzureCrypto : ICrypto
    {
        private readonly ISymmetricKeyCache _keyStore;

        public AzureCrypto(ISymmetricKeyCache store)
        {
            _keyStore = store;
        }

        public string EncryptStringAndBase64(string s)
        {
            return EncryptStringAndBase64(s, null);
        }

        public string EncryptStringAndBase64(string s, Guid? userId)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            var cryptedBytes = Encrypt(bytes, userId);
            return Convert.ToBase64String(cryptedBytes);
        }

        public string DecryptStringFromBase64(string base64String)
        {
            return DecryptStringFromBase64(base64String, null);
        }

        public string DecryptStringFromBase64(string base64String, Guid? userId)
        {
            var bytes = Decrypt(Convert.FromBase64String(base64String), userId);
            return Encoding.Unicode.GetString(bytes);
        }

        public byte[] Encrypt(byte[] bytes)
        {
            return Encrypt(bytes, null);
        }

        public byte[] Encrypt(byte[] bytes, Guid? userId)
        {
            using (var msEncrypted = new MemoryStream())
            {
                using (var encryptor = _keyStore.GetEncryptor(userId))
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

        public byte[] Decrypt(byte[] bytes)
        {
            return Decrypt(bytes, null);
        }

        public byte[] Decrypt(byte[] bytes, Guid? userId)
        {
            using (var decryptor = _keyStore.GetDecryptor(userId))
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

        public ICryptoTransform GetEncryptor()
        {
            return GetEncryptor(null);
        }

        public ICryptoTransform GetEncryptor(Guid? userId)
        {
            return _keyStore.GetEncryptor(userId);
        }

        public ICryptoTransform GetDecryptor()
        {
            return GetDecryptor(null);
        }

        public ICryptoTransform GetDecryptor(Guid? userId)
        {
            return _keyStore.GetDecryptor(userId);
        }
    }
}
