namespace Azure.Security.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Caching;
    using Exceptions;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;

    [TestClass]
    [DeploymentItem(@"TestFiles\TestCertificate.pfx")]
    public class SymmetricKeyTableManagerTests
    {
        private const string TableName = "RandomTableName";
        private static readonly Guid TestUserId = new Guid("e6f41e92-a89f-47ab-b511-224260f3bb55");
        private readonly CloudStorageAccount acct = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
        private static RsaHelper rsaHelper;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            var deploymentDirectory = TestContext.DeploymentDirectory;
            rsaHelper = new RsaHelper(Path.Combine(deploymentDirectory, "TestCertificate.pfx"), "test");
        }

        [TestCleanup]
        public void TestTearDown()
        {
            var client = acct.CreateCloudTableClient();
            var table = client.GetTableReference(TableName);
            table.DeleteIfExists();
            MemoryCache.Default.Dispose();
        }

        [TestMethod]
        public void ConstructorShouldInitializeSuccessfully()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.Should().NotBeNull("Initialization failed.");
        }

        [TestMethod]
        public void GetKeyShouldReturnNull()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var key = symmetricTableManager.GetKey(null);

            key.Should().BeNull("The get query did not return null as expected");
        }

        [TestMethod]
        public void GetKeyShouldThrowAnException()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);

            Action action = () => symmetricTableManager.GetKey(null);
            action.ShouldThrow<AzureCryptoException>();
        }

        [TestMethod]
        public void GetKeyShouldReturnOneKey()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset(null);
            symmetricTableManager.AddSymmetricKey(newKey);

            var key = symmetricTableManager.GetKey(null);

            key.Should().NotBeNull("The get query failed");
        }

        [TestMethod]
        public void GetKeyShouldReturnOneKeyWithUserId()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset(TestUserId);
            symmetricTableManager.AddSymmetricKey(newKey);

            var key = symmetricTableManager.GetKey(TestUserId);

            key.Should().NotBeNull("The get query failed");
        }

        [TestMethod]
        public void DeleteKeyShouldSucceed()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset(null);
            symmetricTableManager.AddSymmetricKey(newKey);

            var key = symmetricTableManager.GetKey(null);
            key.Should().NotBeNull("Insert operation failed");

            symmetricTableManager.DeleteSymmetricKey(newKey);
            key = symmetricTableManager.GetKey(null);
            key.Should().BeNull("Delete operation failed");
        }

        [TestMethod]
        public void DeleteKeyShouldSucceedWithUserId()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset(TestUserId);
            symmetricTableManager.AddSymmetricKey(newKey);

            var key = symmetricTableManager.GetKey(TestUserId);
            key.Should().NotBeNull("Insert operation failed");

            symmetricTableManager.DeleteSymmetricKey(newKey);
            key = symmetricTableManager.GetKey(TestUserId);
            key.Should().BeNull("Delete operation failed");
        }
    }
}
