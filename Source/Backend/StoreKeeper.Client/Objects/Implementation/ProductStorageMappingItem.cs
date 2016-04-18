using CommonBase;

using StoreKeeper.Client.Objects.DataProxy;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ProductStorageMappingItem : BaseObject<ProductStorageMappingItemDataProxy>, IProductStorageMappingItem
    {
        public ProductStorageMappingItem(ProductStorageMappingItemDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IProductStorageMappingItem Implementation

        public ObjectId ItemId
        {
            get { return Proxy.ArticleId; }
        }

        public string Code
        {
            get { return Proxy.Code; }
        }

        public ArticleType Type
        {
            get { return Proxy.Type; }
        }

        public string Name
        {
            get { return Proxy.Name; }
        }

        public string Storage
        {
            get { return Proxy.Storage; }
        }

        public ObjectId StorageId
        {
            get { return Proxy.StorageId; }
            set { Proxy.StorageId = value; }
        }

        public bool SkipCalculation
        {
            get { return Proxy.SkipCalculation; }
            set { Proxy.SkipCalculation = value; }
        }

        #endregion
    }
}