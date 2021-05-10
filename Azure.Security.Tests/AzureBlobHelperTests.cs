namespace Azure.Security.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;

    [TestClass]
    [DeploymentItem(@"TestFiles\TestCertificate.pfx")]
    public class AzureBlobHelperTests
    {
        private const string DirectoryName = "testdirectory";
        private static readonly string ContainerName = Guid.NewGuid().ToString("N").ToLower();
        private static readonly string BlobId = Guid.NewGuid().ToString("N");
        private static readonly string ConnectionString = "UseDevelopmentStorage=true";
        private const string TestString = "This is some random string used for this test";

        [ClassCleanup]
        public static void TearDown()
        {
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            helper.DeleteBlobContainer(ContainerName);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            var blobsToDelete = helper.GetBlobItemsByDirectory(ContainerName);
            foreach (var blob in blobsToDelete)
            {
                blob.DeleteIfExists();
            }
        }

        [TestMethod]
        public void BlobHelperInitializationShouldSucceed()
        {
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            helper.Should().NotBeNull("Initialization has failed");
        }

        [TestMethod]
        public void BlobHelperAddOrCreateBlobShouldSucceed()
        {
            var stream = Serializer.SerializeToByteArray(TestString);
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            helper.CreateOrUpdate(BlobId, stream);
            stream.Close();

            var createdSuccessfully = helper.Exists(BlobId);
            createdSuccessfully.Should().BeTrue("The blob failed to be created or uploaded");
        }

        [TestMethod]
        public void BlobHelperAddOrCreateDirectoryBlobShouldSucceed()
        {
            var stream = Serializer.SerializeToByteArray(TestString);
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            var blobPath = Path.Combine(DirectoryName, BlobId);
            helper.CreateOrUpdate(blobPath, stream);
            stream.Close();

            var createdSuccessfully = helper.Exists(BlobId);
            createdSuccessfully.Should().BeTrue("The blob failed to be created or uploaded");
        }

        [TestMethod]
        public void BlobHelperDeleteBlobShouldSucceed()
        {
            var serializedKey = Serializer.SerializeToByteArray(TestString);
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            helper.CreateOrUpdate(BlobId, serializedKey);

            var createdSuccessfully = helper.Exists(BlobId);
            createdSuccessfully.Should().BeTrue("The create or upload operation failed");

            helper.Delete(BlobId);

            var blobDoesExist = helper.Exists(BlobId);
            blobDoesExist.Should().BeFalse("The delete operation failed");
        }

        [TestMethod]
        public void BlobHelperExistsShouldReturnFalseForInexistentBlob()
        {
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            var blobExists = helper.Exists("RandomId");
            blobExists.Should().BeFalse("No blob with RandomId should exist");
        }

        [TestMethod]
        public void BlobHelperGetShouldReturnTheCorrectObject()
        {
            var serializedKey = Serializer.SerializeToByteArray(TestString);
            var helper = new AzureBlobHelper(ConnectionString, ContainerName);
            helper.CreateOrUpdate(BlobId, serializedKey);

            var createdSuccessfully = helper.Exists(BlobId);
            createdSuccessfully.Should().BeTrue("The create or upload operation failed");

            var stream = helper.Get(BlobId);
            stream.Should().NotBeNull("Failed to get memory stream from blob");

            var deserializedObject = Serializer.DeserializeFromStream(stream);
            deserializedObject.Should().NotBeNull("Object was created successfully");
            
            TestString.Should().BeEquivalentTo(deserializedObject.ToString());
        }
    }
}
