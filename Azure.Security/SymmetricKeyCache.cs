namespace Azure.Security
{
    using Exceptions;
    using Interfaces;
    using System;
    using System.Security.Cryptography;

    public class SymmetricKeyCache : ISymmetricKeyCache
    {
        private SymmetricAlgorithmItem keyCache;
        private readonly ISymmetricKeyTableManager symmetricKeyTableManager;
        private readonly IRsaHelper rsaHelper;

        public SymmetricKeyCache(IRsaHelper theRsaHelper, ISymmetricKeyTableManager keyTableManager, Guid? userId)
        {
            rsaHelper = theRsaHelper;
            symmetricKeyTableManager = keyTableManager;
            Init(userId);
        }

        internal void Init(Guid? userId)
        {
            var key = symmetricKeyTableManager.GetKey(userId);
            
            try
            {
                var symmetricCryptoKey = rsaHelper.RsaDecryptToBytes(key.Key);
                var symmetricCryptoIv = rsaHelper.RsaDecryptToBytes(key.Iv);

                var algorithm = new SymmetricAlgorithmItem
                {
                    Algorithm = new AesManaged {IV = symmetricCryptoIv, Key = symmetricCryptoKey},
                    UserId = key.UserId
                };
                keyCache = algorithm;
            }
            catch (Exception ex)
            {
                throw new AzureCryptoException("Error initializing crypto key.", ex);
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
            if (keyCache.UserId != userId)
            {
                throw new AzureCryptoException($"No keys have been configured. KeyCache UserId: {keyCache.UserId}, userId: {userId}");
            }
            return keyCache.Algorithm;
        }
    }
}
