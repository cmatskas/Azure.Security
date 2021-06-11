using System;

namespace Azure.Security.Interfaces
{
    using Microsoft.Azure.Cosmos.Table;

    public interface ISymmetricKeyTableManager
    {
        SymmetricKey GetKey(Guid? userId);

        void DeleteSymmetricKey(SymmetricKey key);

        void AddSymmetricKey(SymmetricKey key);

        CloudTable CreateTableIfNotExists();

        void DeleteTableIfExists();
    }
}
