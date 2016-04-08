namespace Azure.Security
{
    using Interfaces;
    using Microsoft.WindowsAzure.Storage;
    using System;
    using System.Configuration;
    using System.IO;

    public class EncryptionHelper : IEncryptionHelper
    {
        public CloudStorageAccount StorageAccount { get; set; } 
        public IRsaHelper RsaHelper { get; set; }
        public ISymmetricKeyTableManager KeyTableManager { get; set; }
        public ISymmetricKeyCache KeyCache { get; set; }
        public ICrypto AzureCrypto{ get; set; }

        private readonly string certificatePath;
        private readonly string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
        private readonly string certificateValue = ConfigurationManager.AppSettings["CertificateValue"];
        private readonly string certificateTable = ConfigurationManager.AppSettings["CertificateTable"];
        private readonly string certificateName = ConfigurationManager.AppSettings["CertificateName"];

        public EncryptionHelper(string pathToCertificate) : this(pathToCertificate, null)
        {
            
        }

        public EncryptionHelper(string pathToCertificate, Guid? userId)
        {
            certificatePath = Path.Combine(pathToCertificate,certificateName);
            StorageAccount = CloudStorageAccount.Parse(connectionString);
            RsaHelper = new RsaHelper(certificatePath, certificateValue);
            KeyTableManager = new SymmetricKeyTableManager(certificateTable, StorageAccount);
            
            //Ensure the table is in place before initializing the cryptoStore
            //CreateCertificateTableIfNotExists();
            //Create the master key if it doesn't exist
            CreateNewCryptoKeyIfNotExists(userId);

            KeyCache = new SymmetricKeyCache(RsaHelper, KeyTableManager, userId);
            AzureCrypto = new AzureCrypto(KeyCache);
        }

        public void CreateNewCryptoKeyIfNotExists()
        {
            CreateNewCryptoKeyIfNotExists(null);
        }

        public void CreateNewCryptoKeyIfNotExists(Guid? userId)
        {
            var key = KeyTableManager.GetKey(userId);
            if (key != null)
            {
                return;
            }

            var newKey = RsaHelper.CreateNewAesSymmetricKeyset(userId);
            KeyTableManager.AddSymmetricKey(newKey);
        }

        public byte[] EncryptBytes(byte[] bytesToEncrypt)
        {
            return EncryptBytes(bytesToEncrypt, null);
        }

        public byte[] EncryptBytes(byte[] bytesToEncrypt, Guid? userId, bool createIfNotExists = true)
        {
            // Create the master key if it doesn't exist, if required
            if(createIfNotExists)
                CreateNewCryptoKeyIfNotExists(userId);

            return AzureCrypto.Encrypt(bytesToEncrypt, userId);
        }

        public byte[] DecryptBytes(byte[] bytesToDecrypt)
        {
            return DecryptBytes(bytesToDecrypt, null);
        }

        public byte[] DecryptBytes(byte[] bytesToDecrypt, Guid? userId)
        {
            return AzureCrypto.Decrypt(bytesToDecrypt, userId);
        }

        public string EncryptAndBase64(string valueToEncrypt)
        {
            return EncryptAndBase64(valueToEncrypt, null);
        }

        public string EncryptAndBase64(string valueToEncrypt, Guid? userId, bool createIfNotExists = true)
        {
            // Create the master key if it doesn't exist, if required
            if (createIfNotExists)
                CreateNewCryptoKeyIfNotExists(userId);

            return AzureCrypto.EncryptStringAndBase64(valueToEncrypt, userId);
        }

        public string DecryptFromBase64(string valueToDecrypt)
        {
            return DecryptFromBase64(valueToDecrypt, null);
        }

        public string DecryptFromBase64(string valueToDecrypt, Guid? userId)
        {
            return AzureCrypto.DecryptStringFromBase64(valueToDecrypt, userId);
        }

        private void CreateCertificateTableIfNotExists()
        {
            KeyTableManager.CreateTableIfNotExists();
        }
        
    }
}