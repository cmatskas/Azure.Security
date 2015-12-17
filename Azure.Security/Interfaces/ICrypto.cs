namespace Azure.Security.Interfaces
{
    using System;
    using System.Security.Cryptography;

    public interface ICrypto
    {
        string EncryptStringAndBase64(string s, Guid userId);

        string DecryptStringFromBase64(string base64String, Guid userId);

        byte[] Encrypt(byte[] bytes, Guid userId);

        byte[] Decrypt(byte[] bytes, Guid userId);

        ICryptoTransform GetEncryptor(Guid userId);

        ICryptoTransform GetDecryptor(Guid userId);
    }
}
