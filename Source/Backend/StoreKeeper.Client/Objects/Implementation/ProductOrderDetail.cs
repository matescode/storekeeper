using CommonBase;

using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ProductOrderDetail : BaseObject<ProductOrderDetailDataProxy>, IProductOrderDetail
    {
        public ProductOrderDetail(ProductOrderDetailDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IProductOrderDetail Implementation

        public ObjectId OrderId
        {
            get { return Proxy.OrderId; }
        }

        public ObjectId ProductId
        {
            get { return Proxy.ProductId; }
        }

        public string Name
        {
            get { return Proxy.Name; }
        }

        public ObjectId ItemId
        {
            get { return Proxy.ItemId; }
        }

        public string Title
        {
            get { return Proxy.Title; }
        }

        public string Description
        {
            get { return Proxy.Description; }
        }

        #endregion
    }
}