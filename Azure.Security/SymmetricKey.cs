namespace Azure.Security
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public sealed class SymmetricKey : TableEntity
    {
        public SymmetricKey()
        {
            PartitionKey = "SymmetricKey";
            RowKey = Guid.NewGuid().ToString("N");
            CreateDate = DateTime.UtcNow;
        }

        public byte[] Key { get; set; }
        public byte[] Iv { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid? UserId { get; set; }
    }
}
