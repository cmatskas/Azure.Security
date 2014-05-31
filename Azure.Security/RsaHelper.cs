namespace Azure.Security
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class RsaHelper
    {
        private static readonly UnicodeEncoding ByteConverter = new UnicodeEncoding();
        private X509Certificate2 x509;

        public RsaHelper(string certificatePath, string password)
        {
            CreateX509CertificateFromCerFile(certificatePath, password);
        }

        private void CreateX509CertificateFromCerFile(string certificateFilePath, string password)
        {
            var data = File.ReadAllBytes(certificateFilePath);
            x509 = new X509Certificate2(data, password);
        }

        public byte[] RsaEncryptString(string plainText)
        {
            var dataToEncrypt = ByteConverter.GetBytes(plainText);
            return RsaEncryptBytes(dataToEncrypt);
        }

        public byte[] RsaEncryptBytes(byte[] binaryData)
        {
            var rsa = (RSACryptoServiceProvider) x509.PublicKey.Key;
            return rsa.Encrypt(binaryData, false);
        }

        public byte[] RsaDecryptToBytes(byte[] dataToDecrypt)
        {
            var rsa = (RSACryptoServiceProvider) x509.PrivateKey;
            return rsa.Decrypt(dataToDecrypt, false);
        }

        public string RsaDecryptToString(byte[] dataToDecrypt)
        {
            return ByteConverter.GetString(RsaDecryptToBytes(dataToDecrypt));
        }

        public SymmetricKey CreateNewAesSymmetricKeyset()
        {
            var aes = new AesManaged();
            aes.GenerateIV();
            aes.GenerateKey();

            var symmKeySet = new SymmetricKey()
            {
                Iv = RsaEncryptBytes(aes.IV),
                Key = RsaEncryptBytes(aes.Key)
            };

            return symmKeySet;
        }
    }
}
