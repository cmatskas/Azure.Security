namespace Azure.Security.Interfaces
{
    public interface IRsaHelper
    {
        byte[] RsaEncryptString(string plainText);

        byte[] RsaEncryptBytes(byte[] binaryData);

        byte[] RsaDecryptToBytes(byte[] dataToDecrypt);

        string RsaDecryptToString(byte[] dataToDecrypt);

        SymmetricKey CreateNewAesSymmetricKeyset();
    }
}
