namespace Azure.Security.Interfaces
{
    using System;
    using System.Security.Cryptography;

    public interface ISymmetricKeyCache
    {
        ICryptoTransform GetDecryptor(Guid userId);

        ICryptoTransform GetEncryptor(Guid userId);
    }
}
