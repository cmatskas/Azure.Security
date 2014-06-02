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
        private readonly List<SymmetricAlgorithm> keyCache = new List<SymmetricAlgorithm>();
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

                    var algorithm = new AesManaged { IV = symmetricCryptoIv, Key = symmetricCryptoKey };
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
            return GetAlgorithm().CreateDecryptor();
        }

        public ICryptoTransform GetEncryptor()
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
