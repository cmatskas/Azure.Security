namespace Azure.Security.Interfaces
{
    using Azure.Storage.Blobs;
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

        IEnumerable<BlobClient> GetBlobItemsByDirectory(string directoryName);

        bool Exists(string blobId);
    }
}
