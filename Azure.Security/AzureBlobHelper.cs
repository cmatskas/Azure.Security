namespace Azure.Security
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class AzureBlobHelper : IBlobHelper
    {
        private static BlobServiceClient _blobServiceClient;
        private static BlobContainerClient _blobContainerClient;
        private static string _containerName;

        public AzureBlobHelper(string connectionString, string blobContainerName)
        {
            if (NameContainsUpperCaseCharacters(blobContainerName))
            {
                throw new ArgumentException("The blob container name has upper case characters or spaces");
            }

            _containerName = blobContainerName;
            _blobServiceClient = new BlobServiceClient(connectionString);
            InitializeStorageAccountAndContainer();
        }

        public void DeleteBlobContainer(string toDelete = null)
        {
            var containerToDelete = !string.IsNullOrEmpty(toDelete) ? toDelete : _containerName;

            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerToDelete);

            _blobContainerClient.DeleteIfExists();
        }

        public void Delete(string blobId)
        {
            var blob = _blobContainerClient.GetBlobClient(blobId);

            blob.DeleteIfExists();
        }

        public void CreateOrUpdate(string blobId, MemoryStream contentStream)
        {
            var blob = _blobContainerClient.GetBlobClient(blobId);

            blob.Upload(contentStream);
        }

        public void CreateOrUpdate(string blobId, Stream contentStream)
        {
            var blob = _blobContainerClient.GetBlobClient(blobId);
            blob.Upload(contentStream);
        }

        public BlobClient CreateOrUpdate(string blobId, Stream contentStream, string contentType)
        {
            var blob = _blobContainerClient.GetBlobClient(blobId);
            var blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };

            blob.Upload(contentStream, blobHttpHeader);

            return blob;
        }

        public MemoryStream Get(string blobId)
        {
            var blob = _blobContainerClient.GetBlobClient(blobId);
            var memoryStream = new MemoryStream();
            blob.DownloadTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        public IEnumerable<BlobClient> GetBlobItemsByDirectory(string directoryName)
        {
            var flattenBlobs = _blobContainerClient.GetBlobs().ToList();
            return flattenBlobs.Where(x => x.Name.Contains(directoryName)).Select(x => _blobContainerClient.GetBlobClient(x.Name));
        }

        public bool Exists(string blobId)
        {
            var flattenBlobs = _blobContainerClient.GetBlobs();
            return CheckBlobExists(flattenBlobs, blobId);
        }

        private bool CheckBlobExists(IEnumerable<BlobItem> blobList, string blobId)
        {
            foreach (var blockBlob in blobList)
            {
                if (blockBlob == null)
                {
                    continue;
                }

                if (blockBlob.Name.Contains(blobId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool NameContainsUpperCaseCharacters(string stringToValidate)
        {
            return !string.IsNullOrEmpty(stringToValidate) && stringToValidate.Any(char.IsUpper);
        }

        private static void InitializeStorageAccountAndContainer()
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            _blobContainerClient.CreateIfNotExists();
        }
    }
}
