namespace Azure.Security.Interfaces
{
    using System;
    using System.Security.Cryptography;

    public interface ISymmetricKeyCache
    {
        ICryptoTransform GetDecryptor();

        ICryptoTransform GetDecryptor(Guid? userId);

        ICryptoTransform GetEncryptor();

        ICryptoTransform GetEncryptor(Guid? userId);
    }
}
