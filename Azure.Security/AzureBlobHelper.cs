namespace Azure.Security
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class AzureBlobHelper
    {
        private static CloudStorageAccount storageAccount;
        private static CloudBlobContainer container;
        private static string containerName;

        public AzureBlobHelper(CloudStorageAccount account, string blobContainerName)
        {
            if (NameContailsUpperCaseCharacters(blobContainerName))
            {
                throw new ArgumentException("The blob container name has upper case characters or spaces");
            }

            containerName = blobContainerName;
            storageAccount = account;
            InitializeStorageAcccountAndContainer();
        }

        private static void InitializeStorageAcccountAndContainer()
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference(containerName);

            container.CreateIfNotExists();
        }

        public void DeleteBlobContainer(string toDelete = null)
        {
            var containerToDelete = !string.IsNullOrEmpty(toDelete) ? toDelete : containerName;
            
            var blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference(containerToDelete);

            container.DeleteIfExists();
        }

        public void Delete(string blobId)
        {
            var blob = container.GetBlockBlobReference(blobId);

            blob.DeleteIfExists();
        }

        public void CreateOrUpdate(string blobId, MemoryStream contentStream)
        {
            var blob = container.GetBlockBlobReference(blobId);

            blob.UploadFromStream(contentStream);
        }

        public void CreateOrUpdate(string blobId, Stream contentStream)
        {
            var blob = container.GetBlockBlobReference(blobId);
            blob.UploadFromStream(contentStream);
        }

        public CloudBlockBlob CreateOrUpdate(string blobId, Stream contentStream, string contentType)
        {
            var blob = container.GetBlockBlobReference(blobId);
            blob.Properties.ContentType = contentType;
            blob.UploadFromStream(contentStream);

            return blob;
        }

        public MemoryStream Get(string blobId)
        {
            var blob = container.GetBlockBlobReference(blobId);
            var memoryStream = new MemoryStream();
            blob.DownloadToStream(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
                
            return memoryStream;
        }

        public IEnumerable<IListBlobItem> GetBlobItemsByDirectory(string directoryName)
        {
            var flattenBlobs = container.ListBlobs(null, true).ToList();
            return flattenBlobs.Where(x => x.StorageUri.PrimaryUri.ToString().Contains(directoryName));
        } 

        public bool Exists( string blobId)
        {
            var flattenBlobs = container.ListBlobs(null, true);
            return CheckBlobExists(flattenBlobs, blobId);
        }

        private bool CheckBlobExists(IEnumerable<IListBlobItem> blobList, string blobId)
        {
            foreach(var blobItem in blobList)
            { 
                var blockBlob = blobItem as CloudBlockBlob;
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

        private static bool NameContailsUpperCaseCharacters(string stringToValidate)
        {
            return !string.IsNullOrEmpty(stringToValidate) && stringToValidate.Any(char.IsUpper);
        }
    }
}
