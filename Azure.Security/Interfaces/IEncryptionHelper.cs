namespace Azure.Security.Interfaces
{
    public interface IEncryptionHelper
    {
        void CreateNewCryptoKeyIfNotExists();

        byte[] EncryptBytes(byte[] bytesToEncrypt);

        byte[] DecryptBytes(byte[] bytesToDecrypt);

        string EncryptAndBase64(string valueToEncrypt);

        string DecryptFromBase64(string valueToDecrypt);
    }
}
