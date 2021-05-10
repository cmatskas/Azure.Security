namespace Azure.Security
{
    using Exceptions;
    using Interfaces;
    using System;
    using System.Security.Cryptography;

    public class SymmetricKeyCache : ISymmetricKeyCache
    {
        private SymmetricAlgorithmItem _keyCache;
        private readonly ISymmetricKeyTableManager _symmetricKeyTableManager;
        private readonly IRsaHelper _rsaHelper;

        public SymmetricKeyCache(IRsaHelper theRsaHelper, ISymmetricKeyTableManager keyTableManager, Guid? userId)
        {
            _rsaHelper = theRsaHelper;
            _symmetricKeyTableManager = keyTableManager;
            Init(userId);
        }

        internal void Init(Guid? userId)
        {
            var key = _symmetricKeyTableManager.GetKey(userId);
            
            try
            {
                var symmetricCryptoKey = _rsaHelper.RsaDecryptToBytes(key.Key);
                var symmetricCryptoIv = _rsaHelper.RsaDecryptToBytes(key.Iv);

                var algorithm = new SymmetricAlgorithmItem
                {
                    Algorithm = new AesManaged {IV = symmetricCryptoIv, Key = symmetricCryptoKey},
                    UserId = key.UserId
                };
                _keyCache = algorithm;
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
            if (_keyCache.UserId != userId)
            {
                throw new AzureCryptoException($"No keys have been configured. KeyCache UserId: {_keyCache.UserId}, userId: {userId}");
            }
            return _keyCache.Algorithm;
        }
    }
}
