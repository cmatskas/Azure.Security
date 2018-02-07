namespace Azure.Security.Interfaces
{
    using System;

    public interface IEncryptionHelper
    {
        void CreateNewCryptoKeyIfNotExists();

        void CreateNewCryptoKeyIfNotExists(Guid? userId);

        byte[] EncryptBytes(byte[] bytesToEncrypt);

        byte[] EncryptBytes(byte[] bytesToEncrypt, Guid? userId, bool createIfNotExists);

        byte[] DecryptBytes(byte[] bytesToDecrypt);

        byte[] DecryptBytes(byte[] bytesToDecrypt, Guid? userId);

        string EncryptAndBase64(string valueToEncrypt);

        string EncryptAndBase64(string valueToEncrypt, Guid? userId, bool createIfNotExists);

        string DecryptFromBase64(string valueToDecrypt);

        string DecryptFromBase64(string valueToDecrypt, Guid? userId);
    }
}
