using CommonBase;
using StoreKeeper.Client.Objects.DataProxy;
using StoreKeeper.Common.DataContracts;
using StoreKeeper.Common.DataContracts.Accounting;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ProductOrderItem : BaseObject<ProductOrderItemDataProxy>, IProductOrderItem
    {
        public ProductOrderItem(ProductOrderItemDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IProductOrderItem Implementation

        public ObjectId ItemId
        {
            get { return Proxy.ItemId; }
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

        public double Count
        {
            get { return Proxy.Count; }
        }

        public double StockAvailable
        {
            get { return Proxy.StockAvailable; }
        }

        public string Storage
        {
            get { return Proxy.Storage; }
        }

        public double ProductionReservation
        {
            get { return Proxy.ProductionReservation; }
        }

        public double OrderCount
        {
            get { return Proxy.OrderCount; }
        }

        public IMaterialOrderStatus MaterialOrderStatus
        {
            get { return Proxy.MaterialOrderStatus; }
        }

        #endregion
    }
}