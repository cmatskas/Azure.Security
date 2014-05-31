namespace Azure.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Exceptions;

    public class SymmetricKeyCache
    {
        private readonly List<SymmetricAlgorithm> keyCache = new List<SymmetricAlgorithm>();

        public SymmetricKeyCache(RsaHelper rsaHelper, SymmetricKeyTableManager keyTableManager)
        {
            var allKeys = keyTableManager.GetAllKeys();

            foreach (var key in allKeys)
            {
                try
                {
                    var symmetricCryptoKey = rsaHelper.RsaDecryptToBytes(key.Key);
                    var symmetricCryptoIv = rsaHelper.RsaDecryptToBytes(key.Iv);

                    var algorithm = new AesManaged {IV = symmetricCryptoIv, Key = symmetricCryptoKey};
                    keyCache.Add(algorithm);
                }
                catch (Exception ex)
                {
                    throw new AzureCryptoException("Error initializing crypto key.", ex);
                }
            }
        }

        internal ICryptoTransform GetDecryptor()
        {
            return GetAlgorithm().CreateDecryptor();
        }

        internal ICryptoTransform GetEncryptor()
        {
            return GetAlgorithm().CreateEncryptor();
        }

        private SymmetricAlgorithm GetAlgorithm()
        {
            if (!keyCache.Any())
            {
                throw new AzureCryptoException("No keys have been configured.");
            }
            return keyCache.First();
        }
    }
}
