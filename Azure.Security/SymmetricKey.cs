namespace Azure.Security
{
    using Microsoft.Azure.Cosmos.Table;
    using System;

    public sealed class SymmetricKey : TableEntity
    {
        // Parameter less constructor for table queries
        public SymmetricKey() { }

        // Use the user id as the row key for faster lookup
        public SymmetricKey(Guid? userId)
        {
            PartitionKey = "SymmetricKey";
            RowKey = userId?.ToString("N") ?? Guid.Empty.ToString("N");
            CreateDate = DateTime.UtcNow;
        }

        public byte[] Key { get; set; }
        public byte[] Iv { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid? UserId { get; set; }
    }
}
