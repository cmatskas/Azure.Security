namespace Azure.Security
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using Exceptions;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class SymmetricKeyTableManager
    {
        private static string keyTableName;
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;

        public SymmetricKeyTableManager(string tableName, CloudStorageAccount acct)
        {
            keyTableName = tableName;
            storageAccount = acct;
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public IEnumerable<SymmetricKey> GetAllKeys()
        {
            // Create the CloudTable object that represents the "people" table.
            var table = tableClient.GetTableReference(keyTableName);

            // Create a retrieve operation that takes a customer entity.
            var query = new TableQuery<SymmetricKey>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "SymmetricKey"));

            try
            {
                return table.ExecuteQuery(query).ToList();
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
