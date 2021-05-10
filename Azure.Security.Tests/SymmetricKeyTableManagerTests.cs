namespace Azure.Security.Tests
{
    using Exceptions;
    using FluentAssertions;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Runtime.Caching;

    [TestClass]
    [DeploymentItem(@"TestFiles\TestCertificate.pfx")]
    public class SymmetricKeyTableManagerTests
    {
        private const string TableName = "RandomTableName";
        private static readonly Guid TestUserId = new Guid("e6f41e92-a89f-47ab-b511-224260f3bb55");
        private readonly CloudStorageAccount _acct = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
        private static RsaHelper _rsaHelper;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            var deploymentDirectory = TestContext.DeploymentDirectory;
            _rsaHelper = new RsaHelper(Path.Combine(deploymentDirectory, "TestCertificate.pfx"), "test");
        }

        [TestCleanup]
        public void TestTearDown()
        {
            var client = _acct.CreateCloudTableClient();
            var table = client.GetTableReference(TableName);
            table.DeleteIfExists();
            MemoryCache.Default.Dispose();
        }

        [TestMethod]
        public void ConstructorShouldInitializeSuccessfully()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, _acct);
            symmetricTableManager.Should().NotBeNull("Initialization failed.");
        }

        [TestMethod]
        public void GetKeyShouldReturnNull()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, _acct);
            symmetricTableManager.CreateTableIfNotExists();
            var key = symmetricTableManager.GetKey(null);

            key.Should().BeNull("The get query did not return null as expected");
        }

        [TestMethod]
        public void GetKeyShouldThrowAnException()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, _acct);

            Action action = () => symmetricTableManager.GetKey(null);
            action.Should().Throw<AzureCryptoException>();
        }

        [TestMethod]
        public void GetKeyShouldReturnOneKey()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, _acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = _rsaHelper.CreateNewAesSymmetricKeyset(null);
            symmetricTableManager.AddSymmetricKey(newKey);

            var key = symmetricTableManager.GetKey(null);

            key.Should().NotBeNull("The get query failed");
        }

        [TestMethod]
        public void GetKeyShouldReturnOneKeyWithUserId()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, _acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = _rsaHelper.CreateNewAesSymmetricKeyset(TestUserId);
            symmetricTableManager.AddSymmetricKey(newKey);

            var key = symmetricTableManager.GetKey(TestUserId);

            key.Should().NotBeNull("The get query failed");
        }

        [TestMethod]
        public void DeleteKeyShouldSucceed()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, _acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = _rsaHelper.CreateNewAesSymmetricKeyset(null);
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
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, _acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = _rsaHelper.CreateNewAesSymmetricKeyset(TestUserId);
            symmetricTableManager.AddSymmetricKey(newKey);

            var key = symmetricTableManager.GetKey(TestUserId);
            key.Should().NotBeNull("Insert operation failed");

            symmetricTableManager.DeleteSymmetricKey(newKey);
            key = symmetricTableManager.GetKey(TestUserId);
            key.Should().BeNull("Delete operation failed");
        }
    }
}
