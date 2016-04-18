using System.Collections.Generic;

using CommonBase;

using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ProductStorageMapping : BaseObject<ProductStorageMappingDataProxy>, IProductStorageMapping
    {
        private readonly List<IProductStorageMappingItem> _productItems;

        public ProductStorageMapping(ProductStorageMappingDataProxy dataProxy)
            : base(dataProxy)
        {
            _productItems = new List<IProductStorageMappingItem>();
            Reload();
        }

        #region IProductOrderDetail Implementation

        public ObjectId ProductId
        {
            get { return Proxy.ProductId; }
        }

        public string Name
        {
            get { return Proxy.Name; }
        }

        public IEnumerable<IProductStorageMappingItem> Items
        {
            get { return _productItems; }
        }

        public void Reload()
        {
            _productItems.Clear();
            foreach (ProductStorageMappingItemDataProxy itemProxy in Proxy.ItemProxies)
            {
                _productItems.Add(new ProductStorageMappingItem(itemProxy));
            }
        }

        #endregion
    }
}