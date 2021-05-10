namespace Azure.Security.Interfaces
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using System.Collections.Generic;
    using System.IO;

    public interface IBlobHelper
    {
        void DeleteBlobContainer(string toDelete = null);

        void Delete(string blobId);

        void CreateOrUpdate(string blobId, MemoryStream contentStream);

        void CreateOrUpdate(string blobId, Stream contentStream);

        BlobClient CreateOrUpdate(string blobId, Stream contentStream, string contentType);

        MemoryStream Get(string blobId);

        IEnumerable<BlobItem> GetBlobItemsByDirectory(string directoryName);

        bool Exists(string blobId);
    }
}
