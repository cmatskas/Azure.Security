namespace Azure.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Exceptions;
    using Interfaces;

    public class SymmetricKeyCache : ISymmetricKeyCache
    {
        private readonly List<SymmetricAlgorithmItem> keyCache = new List<SymmetricAlgorithmItem>();
        private readonly ISymmetricKeyTableManager symmetricKeyTableManager;
        private readonly IRsaHelper rsaHelper;

        public SymmetricKeyCache(IRsaHelper theRsaHelper, ISymmetricKeyTableManager keyTableManager)
        {
            rsaHelper = theRsaHelper;
            symmetricKeyTableManager = keyTableManager;
            Init();
        }

        internal void Init()
        {
            var allKeys = symmetricKeyTableManager.GetAllKeys();

            foreach (var key in allKeys)
            {
                try
                {
                    var symmetricCryptoKey = rsaHelper.RsaDecryptToBytes(key.Key);
                    var symmetricCryptoIv = rsaHelper.RsaDecryptToBytes(key.Iv);

                    var algorithm = new SymmetricAlgorithmItem
                    {
                        Algorithm = new AesManaged {IV = symmetricCryptoIv, Key = symmetricCryptoKey},
                        UserId = key.UserId
                    };
                    keyCache.Add(algorithm);
                }
                catch (Exception ex)
                {
                    throw new AzureCryptoException("Error initializing crypto key.", ex);
                }
            }
        }

        public ICryptoTransform GetDecryptor()
        {
            return GetDecryptor(null);
        }

        public ICryptoTransform GetDecryptor(Guid? userId)
        {
            return GetAlgorithm(userId).CreateDecryptor();
        }

        public ICryptoTransform GetEncryptor()
        {
            return GetEncryptor(null);
        }

        public ICryptoTransform GetEncryptor(Guid? userId)
        {
            return GetAlgorithm(userId).CreateEncryptor();
        }

        private SymmetricAlgorithm GetAlgorithm(Guid? userId)
        {
            if (keyCache.All(x => x.UserId != userId))
            {
                throw new AzureCryptoException("No keys have been configured.");
            }
            return keyCache.Single(x => x.UserId == userId).Algorithm;
        }
    }
}
