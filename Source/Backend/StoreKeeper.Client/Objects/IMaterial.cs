using CommonBase;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.Client.Objects
{
    public interface IMaterial : IClientObject, ILoadable
    {
        ObjectId MaterialId { get; }

        string Code { get; }

        string SupplierCode { get; }

        ArticleType Type { get; }

        string Name { get; }

        double Price { get; }

        double CurrentCount { get; }

        double MissingInOrders { get; }

        double OrderedCount { get; }

        int ProductCount { get; }

        double ExternStorageCount { get; }
    }
}