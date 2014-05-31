namespace Azure.Security
{
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage;

    public class EncryptionHelper
    {
        public CloudStorageAccount StorageAccount { get; set; } 
        public RsaHelper RsaHelper { get; set; }
        public SymmetricKeyTableManager KeyTableManager { get; set; }
        public SymmetricKeyCache KeyCache { get; set; }
        public AzureCrypto AzureCrypto{ get; set; }

        private readonly string certificatePath;
        private readonly string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
        private readonly string certificateValue = ConfigurationManager.AppSettings["CertificateValue"];
        private readonly string certificateTable = ConfigurationManager.AppSettings["CertificateTable"];
        private readonly string certificateName = ConfigurationManager.AppSettings["CertificateName"];


        public EncryptionHelper(string pathToCertificate)
        {
            certificatePath = Path.Combine(pathToCertificate,certificateName);
            StorageAccount = CloudStorageAccount.Parse(connectionString);
            RsaHelper = new RsaHelper(certificatePath, certificateValue);
            KeyTableManager = new SymmetricKeyTableManager(certificateTable, StorageAccount);
            //Ensure the table is in place before initializing the cryptoStore
            CreateCertificateTableIfNotExists();

            KeyCache = new SymmetricKeyCache(RsaHelper, KeyTableManager);
            AzureCrypto = new AzureCrypto(KeyCache);
        }

        public void CreateNewCryptoKeyIfNotExists()
        {
            var allKeys = KeyTableManager.GetAllKeys().ToList();
            if (allKeys.Any())
            {
                return;
            }

            var newKey = RsaHelper.CreateNewAesSymmetricKeyset();
            KeyTableManager.AddSymmetricKey(newKey);
        }

        public byte[] EncryptBytes(byte[] bytesToEncrypt)
        {
            return AzureCrypto.Encrypt(bytesToEncrypt);
        }

        public byte[] DecryptBytes(byte[] bytesToDecrypt)
        {
            return AzureCrypto.Decrypt(bytesToDecrypt);
        }

        public string EncryptAndBase64(string valueToEncrypt)
        {
            return AzureCrypto.EncryptStringAndBase64(valueToEncrypt);
        }

        public string DecryptFromBase64(string valueToDecrypt)
        {
            return AzureCrypto.DecryptStringFromBase64(valueToDecrypt);
        }

        private void CreateCertificateTableIfNotExists()
        {
            KeyTableManager.CreateTableIfNotExists();
        }
        
    }
}