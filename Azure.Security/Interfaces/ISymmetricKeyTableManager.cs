namespace Azure.Security.Interfaces
{
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Table;

    public interface ISymmetricKeyTableManager
    {
        IEnumerable<SymmetricKey> GetAllKeys();

        void DeleteSymmetricKey(SymmetricKey key);

        void AddSymmetricKey(SymmetricKey key);

        CloudTable CreateTableIfNotExists();

        void DeleteTableIfExists();
    }
}
