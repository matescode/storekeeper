using CommonBase;

namespace StoreKeeper.Client.Objects
{
    public interface IExternStorageMaterial : IClientObject, ILoadable
    {
        ObjectId StatId { get; }

        ObjectId MaterialId { get; }

        ObjectId StorageId { get; }

        string Code { get; }

        string Name { get; }

        double CurrentCount { get; }

        string Company { get; }

        double CentralStorageCount { get; }

        double MissingCount { get; }

        string SpecialCode { get; }
    }
}