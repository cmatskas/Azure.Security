namespace Azure.Security.Interfaces
{
    using System.Security.Cryptography;

    public interface ICrypto
    {
        string EncryptStringAndBase64(string s);

        string DecryptStringFromBase64(string base64String);

        byte[] Encrypt(byte[] bytes);

        byte[] Decrypt(byte[] bytes);

        ICryptoTransform GetEncryptor();

        ICryptoTransform GetDecryptor();
    }
}
