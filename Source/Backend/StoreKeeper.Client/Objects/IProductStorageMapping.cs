using System.Collections.Generic;
using CommonBase;

namespace StoreKeeper.Client.Objects
{
    public interface IProductStorageMapping
    {
        ObjectId ProductId { get; }

        string Name { get; }

        IEnumerable<IProductStorageMappingItem> Items { get; }

        void Reload();
    }
}