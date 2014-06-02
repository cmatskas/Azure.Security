namespace Azure.Security.Interfaces
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.WindowsAzure.Storage.Blob;

    public interface IBlobHelper
    {
        void DeleteBlobContainer(string toDelete = null);

        void Delete(string blobId);

        void CreateOrUpdate(string blobId, MemoryStream contentStream);

        void CreateOrUpdate(string blobId, Stream contentStream);

        CloudBlockBlob CreateOrUpdate(string blobId, Stream contentStream, string contentType);

        MemoryStream Get(string blobId);

        IEnumerable<IListBlobItem> GetBlobItemsByDirectory(string directoryName);

        bool Exists(string blobId);
    }
}
