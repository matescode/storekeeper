using CommonBase;
using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ExternStorageMaterial : BaseObject<ExternStorageMaterialDataProxy>, IExternStorageMaterial
    {
        public ExternStorageMaterial(ExternStorageMaterialDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IExternStorageMaterial Implementation

        public ObjectId StatId
        {
            get { return Proxy.StatId; }
        }

        public ObjectId MaterialId
        {
            get { return Proxy.MaterialId; }
        }

        public ObjectId StorageId
        {
            get { return Proxy.StorageId; }
        }

        public string Code
        {
            get { return Proxy.Code; }
        }

        public string Name
        {
            get { return Proxy.Name; }
        }

        public double CurrentCount
        {
            get { return Proxy.CurrentCount; }
        }

        public string Company
        {
            get { return Proxy.Company; }
        }

        public double CentralStorageCount
        {
            get { return Proxy.CentralStorageCount; }
        }

        public double MissingCount
        {
            get { return Proxy.MissingCount; }
        }

        public string SpecialCode
        {
            get { return Proxy.SpecialCode; }
        }

        #endregion
    }
}