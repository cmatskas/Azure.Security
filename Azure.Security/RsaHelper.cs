namespace Azure.Security
{
    using Interfaces;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class RsaHelper : IRsaHelper
    {
        private static readonly UnicodeEncoding ByteConverter = new UnicodeEncoding();
        private X509Certificate2 _x509;

        public RsaHelper(string certificatePath, string password)
        {
            CreateX509CertificateFromCerFile(certificatePath, password, X509KeyStorageFlags.MachineKeySet);
        }

        public RsaHelper(string certificatePath, string password, X509KeyStorageFlags flag)
        {
            CreateX509CertificateFromCerFile(certificatePath, password, flag);
        }

        private void CreateX509CertificateFromCerFile(string certificateFilePath, string password, X509KeyStorageFlags flag)
        {
            var data = File.ReadAllBytes(certificateFilePath);
            _x509 = new X509Certificate2(data, password, flag);
        }

        public byte[] RsaEncryptString(string plainText)
        {
            var dataToEncrypt = ByteConverter.GetBytes(plainText);
            return RsaEncryptBytes(dataToEncrypt);
        }

        public byte[] RsaEncryptBytes(byte[] binaryData)
        {
            var rsa = (RSACryptoServiceProvider) _x509.PublicKey.Key;
            return rsa.Encrypt(binaryData, false);
        }

        public byte[] RsaDecryptToBytes(byte[] dataToDecrypt)
        {
            var rsa = (RSACryptoServiceProvider) _x509.PrivateKey;
            return rsa.Decrypt(dataToDecrypt, false);
        }

        public string RsaDecryptToString(byte[] dataToDecrypt)
        {
            return ByteConverter.GetString(RsaDecryptToBytes(dataToDecrypt));
        }

        public SymmetricKey CreateNewAesSymmetricKeyset()
        {
            return CreateNewAesSymmetricKeyset(null);
        }

        public SymmetricKey CreateNewAesSymmetricKeyset(Guid? userId)
        {
            var aes = new AesManaged();
            aes.GenerateIV();
            aes.GenerateKey();

            var symmetricKeySet = new SymmetricKey(userId)
            {
                Iv = RsaEncryptBytes(aes.IV),
                Key = RsaEncryptBytes(aes.Key),
                UserId = userId
            };

            return symmetricKeySet;
        }
    }
}
