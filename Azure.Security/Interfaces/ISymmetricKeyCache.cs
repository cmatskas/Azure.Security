namespace Azure.Security.Interfaces
{
    using System.Security.Cryptography;

    public interface ISymmetricKeyCache
    {
        ICryptoTransform GetDecryptor();

        ICryptoTransform GetEncryptor();
    }
}
