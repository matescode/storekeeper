using CommonBase;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.Client.Objects
{
    public interface IProductStorageMappingItem
    {
        ObjectId ItemId { get; }

        string Code { get; }

        ArticleType Type { get; }

        string Name { get; }

        string Storage { get; }

        ObjectId StorageId { get; set; }

        bool SkipCalculation { get; set; }
    }
}