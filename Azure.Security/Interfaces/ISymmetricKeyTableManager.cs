using System;

namespace Azure.Security.Interfaces
{
    using Microsoft.WindowsAzure.Storage.Table;

    public interface ISymmetricKeyTableManager
    {
        SymmetricKey GetKey(Guid? userId);

        void DeleteSymmetricKey(SymmetricKey key);

        void AddSymmetricKey(SymmetricKey key);

        CloudTable CreateTableIfNotExists();

        void DeleteTableIfExists();
    }
}
