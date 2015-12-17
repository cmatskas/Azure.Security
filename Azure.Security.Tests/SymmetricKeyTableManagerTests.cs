namespace Azure.Security.Tests
{
    using System;
    using System.IO;
    using System.Linq;
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
        }

        [TestMethod]
        public void ConstructorShouldInitializeSuccessfully()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.Should().NotBeNull("Initialization failed.");
        }

        [TestMethod]
        public void GetAllKeysShouldReturnAnEmptyArray()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var allKeys = symmetricTableManager.GetAllKeys().ToList();

            allKeys.Should().NotBeNull("The get query failed");
            allKeys.Count().ShouldBeEquivalentTo(0, "Query returned null or there are items in the table");
        }

        [TestMethod]
        public void GetAllKeysShouldThrowAnException()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);

            Action action = () => symmetricTableManager.GetAllKeys();
            action.ShouldThrow<AzureCryptoException>();
        }

        [TestMethod]
        public void GetAllKeysShouldReturnOneKey()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset(TestUserId);
            symmetricTableManager.AddSymmetricKey(newKey);

            var allKeys = symmetricTableManager.GetAllKeys().ToList();

            allKeys.Should().NotBeNull("The get query failed");
            allKeys.Count().ShouldBeEquivalentTo(1, "Insert operation failed");
        }

        [TestMethod]
        public void DeleteKeyShouldSucceed()
        {
            var symmetricTableManager = new SymmetricKeyTableManager(TableName, acct);
            symmetricTableManager.CreateTableIfNotExists();
            var newKey = rsaHelper.CreateNewAesSymmetricKeyset(TestUserId);
            symmetricTableManager.AddSymmetricKey(newKey);

            var allKeys = symmetricTableManager.GetAllKeys().ToList();
            allKeys.Count().ShouldBeEquivalentTo(1, "Insert operation failed");

            symmetricTableManager.DeleteSymmetricKey(newKey);
            allKeys = symmetricTableManager.GetAllKeys().ToList();
            allKeys.Count().ShouldBeEquivalentTo(0, "Delete operation failed");
        }
    }
}
