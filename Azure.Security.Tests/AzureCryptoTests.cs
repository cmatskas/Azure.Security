namespace Azure.Security.Tests
{
    using System;
    using System.IO;
    using Interfaces;
    using Security;
    using Exceptions;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;

    [TestClass]
    [DeploymentItem(@"TestFiles\TestCertificate.pfx")]
    public class AzureCryptoTests
    {
        private const string TableName = "TestTableName";
        private const string TestString = "This is some test value";
        private readonly CloudStorageAccount acct = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
        private static IRsaHelper rsaHelper;
        private static ISymmetricKeyTableManager tableManager;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestSetup()
        { 
            var deploymentDirectory = TestContext.DeploymentDirectory;
            rsaHelper = new RsaHelper(Path.Combine(deploymentDirectory, "TestCertificate.pfx"), "test");
            tableManager = new SymmetricKeyTableManager(TableName,acct);
            tableManager.CreateTableIfNotExists();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            tableManager.DeleteTableIfExists();
        }

        [TestMethod]
        public void TestAzureTableCryptoInitializesSuccessfully()
        {
            var keyStore = new SymmetricKeyCache(rsaHelper, tableManager );
            var c = new AzureCrypto(keyStore);
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void TestAzureTableCryptoThrowsTableNotFoundException()
        {
            var keyStore = new SymmetricKeyCache(rsaHelper, tableManager);

            var c = new AzureCrypto(keyStore);

            Action action = () => c.GetEncryptor();
            action.ShouldThrow<AzureCryptoException>();
        }

        [TestMethod]
        public void TestAzureTableCryptoHasValidEncryptor()
        {
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset();
            tableManager.AddSymmetricKey(newKey);

            var keyStore = new SymmetricKeyCache(rsaHelper, tableManager);
            var c = new AzureCrypto(keyStore);
            c.Should().NotBeNull("At this stage the contstructor should have succeeded");

            var encryptor = c.GetEncryptor();
            encryptor.Should().NotBeNull("Because the keystore is initialized and there is a key");
        }

        [TestMethod]
        public void EncryptionShouldWorkAsExpected()
        {
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset();
            tableManager.AddSymmetricKey(newKey);

            var keyStore = new SymmetricKeyCache(rsaHelper, tableManager);
            var c = new AzureCrypto(keyStore);

            var encryptedString = c.EncryptStringAndBase64(TestString);
            encryptedString.Should().NotBeNullOrEmpty("Because the encryption failed");
            encryptedString.Should().NotMatch(TestString);
        }

        [TestMethod]
        public void DecryptionShouldReturnTheOriginalString()
        {
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset();
            tableManager.AddSymmetricKey(newKey);

            var keyStore = new SymmetricKeyCache(rsaHelper, tableManager);
            var c = new AzureCrypto(keyStore);

            var encryptedString = c.EncryptStringAndBase64(TestString);
            var decryptedString = c.DecryptStringFromBase64(encryptedString);

            decryptedString.ShouldBeEquivalentTo(TestString);
        }
    }
}
