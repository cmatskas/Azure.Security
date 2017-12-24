namespace Azure.Security.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Caching;
    using Security;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;

    [TestClass]
    [DeploymentItem(@"TestFiles\TestCertificate.pfx")]
    [DeploymentItem(@"TestFiles\TestTextFile.txt")]
    public class EncryptionHelperTests
    {
        public TestContext TestContext { get; set; }
        private static string testFileDeploymentDirectory;
        private const string TestFileName = "TestTextFile.txt";
        private const string TestString = "This is a rendom test string";
        private const string TableName = "TestTableName";
        private static readonly Guid TestUserId = new Guid("e6f41e92-a89f-47ab-b511-224260f3bb55");
        private readonly CloudStorageAccount acct = CloudStorageAccount.Parse("UseDevelopmentStorage=true");

        [TestInitialize]
        public void TestSetup()
        {
            testFileDeploymentDirectory = TestContext.DeploymentDirectory;
            var tableManager = new SymmetricKeyTableManager(TableName, acct);
            tableManager.CreateTableIfNotExists();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory);
            encryptionHelper.KeyTableManager.DeleteTableIfExists();
            MemoryCache.Default.Dispose();
        }

        [TestMethod]
        public void TestConstructorSucceeds()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory);
            encryptionHelper.Should().NotBeNull("Constructor failed");
        }

        [TestMethod]
        public void TestConstructorSucceedsWithUserId()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory, TestUserId);
            encryptionHelper.Should().NotBeNull("Constructor failed");
        }

        [TestMethod]
        public void TestEncryptStringShouldSucceed()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory);
            var encryptedString = encryptionHelper.EncryptAndBase64(TestString);

            encryptedString.Should().NotBeNullOrEmpty("Encryptiong failed");
            encryptedString.Length.Should().BeGreaterThan(0, "Encryption failed");
        }

        [TestMethod]
        public void TestEncryptStringShouldSucceedWithUserId()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory, TestUserId);
            var encryptedString = encryptionHelper.EncryptAndBase64(TestString, TestUserId);

            encryptedString.Should().NotBeNullOrEmpty("Encryptiong failed");
            encryptedString.Length.Should().BeGreaterThan(0, "Encryption failed");
        }

        [TestMethod]
        public void TestDecryptStringShouldSucceed()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory);
            var encryptedString = encryptionHelper.EncryptAndBase64(TestString);
            var decryptedString = encryptionHelper.DecryptFromBase64(encryptedString);

            decryptedString.Should().NotBeNullOrEmpty("Encryptiong failed");
            decryptedString.Length.Should().BeGreaterThan(0, "Encryption failed");
            decryptedString.ShouldBeEquivalentTo(TestString);
        }

        [TestMethod]
        public void TestDecryptStringShouldSucceedWithUserId()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory, TestUserId);
            var encryptedString = encryptionHelper.EncryptAndBase64(TestString, TestUserId);
            var decryptedString = encryptionHelper.DecryptFromBase64(encryptedString, TestUserId);

            decryptedString.Should().NotBeNullOrEmpty("Encryptiong failed");
            decryptedString.Length.Should().BeGreaterThan(0, "Encryption failed");
            decryptedString.ShouldBeEquivalentTo(TestString);
        }

        [TestMethod]
        public void TestEncryptBinaryShouldSucceed()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory);
            var bytesToEncrypt = File.ReadAllBytes(Path.Combine(testFileDeploymentDirectory, TestFileName));
            var encryptedBytes = encryptionHelper.EncryptBytes(bytesToEncrypt);

            encryptedBytes.Should().NotBeNull("EncryptionFailed");
        }

        [TestMethod]
        public void TestEncryptBinaryShouldSucceedWithUserId()
        {
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory, TestUserId);
            var bytesToEncrypt = File.ReadAllBytes(Path.Combine(testFileDeploymentDirectory, TestFileName));
            var encryptedBytes = encryptionHelper.EncryptBytes(bytesToEncrypt, TestUserId);

            encryptedBytes.Should().NotBeNull("EncryptionFailed");
        }

        [TestMethod]
        public void TestDecryptBinaryShouldSucceed()
        {
            var pathToTestFile = Path.Combine(testFileDeploymentDirectory, TestFileName);
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory);
            var bytesToEncrypt = File.ReadAllBytes(pathToTestFile);
            var encryptedBytes = encryptionHelper.EncryptBytes(bytesToEncrypt);

            var decryptedBytes = encryptionHelper.DecryptBytes(encryptedBytes);
            decryptedBytes.Should().NotBeNull("EncryptionFailed");

            var decryptedTestContent = System.Text.Encoding.UTF8.GetString(decryptedBytes);
            var originalContent = File.ReadAllText(pathToTestFile);

            Assert.IsTrue(decryptedTestContent.Equals(originalContent, StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void TestDecryptBinaryShouldSucceedWithUserId()
        {
            var pathToTestFile = Path.Combine(testFileDeploymentDirectory, TestFileName);
            var encryptionHelper = new EncryptionHelper(testFileDeploymentDirectory, TestUserId);
            var bytesToEncrypt = File.ReadAllBytes(pathToTestFile);
            var encryptedBytes = encryptionHelper.EncryptBytes(bytesToEncrypt, TestUserId);

            var decryptedBytes = encryptionHelper.DecryptBytes(encryptedBytes, TestUserId);
            decryptedBytes.Should().NotBeNull("EncryptionFailed");

            var decryptedTestContent = System.Text.Encoding.UTF8.GetString(decryptedBytes);
            var originalContent = File.ReadAllText(pathToTestFile);

            Assert.IsTrue(decryptedTestContent.Equals(originalContent, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
