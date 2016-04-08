namespace Azure.Security
{
    using Exceptions;
    using Interfaces;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Data.Services.Client;
    using System.Linq;

    public class SymmetricKeyTableManager : ISymmetricKeyTableManager
    {
        private static string keyTableName;
        private readonly CloudTableClient tableClient;

        // Cache helper
        private static readonly Cache Cache = Cache.Current;

        public SymmetricKeyTableManager(string tableName, CloudStorageAccount storageAccount)
        {
            keyTableName = tableName;
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public SymmetricKey GetKey(Guid? userId)
        {
            // Construct a unique key
            var itemKey = $"tablekeymanager/key/{userId?.ToString() ?? "none"}";

            // Try to get the item from the cache
            var cachedKey = Cache.GetItem<SymmetricKey>(itemKey);

            // If the data was found in the cache, return it
            if (cachedKey != null)
            {
                return cachedKey;
            }

            // Create the CloudTable object that represents the "key" table.
            var table = tableClient.GetTableReference(keyTableName);
            
            // Create a retrieve operation that takes a customer entity.
            var query = new TableQuery<SymmetricKey>()
                .Where(userId.HasValue
                    ? TableQuery.GenerateFilterConditionForGuid("UserId", QueryComparisons.Equal, userId.Value)
                    : TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, string.Empty));

            // Get the data using the partition and row keys (fastest way to query known data)
            //var operation = TableOperation.Retrieve<SymmetricKey>("SymmetricKey", userId?.ToString("N") ?? Guid.Empty.ToString("N"));

            try
            {
                // Get the key from the table instead
                cachedKey = table.ExecuteQuery(query).FirstOrDefault();

                // Execute the operation
                //var result = table.Execute(operation);

                // If we found the data
                //if (result.Result != null)
                //    cachedKey = (SymmetricKey) result.Result;
            }
            catch (DataServiceQueryException dsq)
            {
                throw new AzureCryptoException("Failed to load encryption keys from storage", dsq);
            }
            catch (DataServiceClientException dsce)
            {
                throw new AzureCryptoException("Failed to load encryption keys from storage", dsce);
            }
            catch (Exception ex)
            {
                throw new AzureCryptoException("Could not load encryption keys table", ex);
            }
            
            // Add the data to the cache for 3 hours if it was found
            if(cachedKey != null)
                Cache.AddItem(itemKey, cachedKey);

            return cachedKey;
        }

        public void DeleteSymmetricKey(SymmetricKey key)
        {
            var cloudTable = GetTableForOperation();

            var deleteOperation = TableOperation.Delete(key);
            cloudTable.Execute(deleteOperation);
        }

        public void AddSymmetricKey(SymmetricKey key)
        {
            var cloudTable = GetTableForOperation();

            var insertOperation = TableOperation.Insert(key);
            cloudTable.Execute(insertOperation);
        }

        public CloudTable CreateTableIfNotExists()
        {
            var cloudTable = tableClient.GetTableReference(keyTableName);
            cloudTable.CreateIfNotExists();

            return cloudTable;
        }

        public void DeleteTableIfExists()
        {
            var table = tableClient.GetTableReference(keyTableName);
            table.DeleteIfExists();
        }

        private CloudTable GetTableForOperation()
        {
            var cloudTable = tableClient.GetTableReference(keyTableName);

            if (cloudTable == null)
            {
                throw new AzureCryptoException(string.Format("Table {0} does not exist", keyTableName));
            }

            return cloudTable;
        }
    }
}
