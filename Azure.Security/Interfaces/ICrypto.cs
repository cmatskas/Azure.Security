namespace Azure.Security.Interfaces
{
    using System;
    using System.Security.Cryptography;

    public interface ICrypto
    {
        string EncryptStringAndBase64(string s);

        string EncryptStringAndBase64(string s, Guid? userId);

        string DecryptStringFromBase64(string base64String);

        string DecryptStringFromBase64(string base64String, Guid? userId);

        byte[] Encrypt(byte[] bytes);

        byte[] Encrypt(byte[] bytes, Guid? userId);

        byte[] Decrypt(byte[] bytes);

        byte[] Decrypt(byte[] bytes, Guid? userId);

        ICryptoTransform GetEncryptor();

        ICryptoTransform GetEncryptor(Guid? userId);

        ICryptoTransform GetDecryptor();

        ICryptoTransform GetDecryptor(Guid? userId);
    }
}
