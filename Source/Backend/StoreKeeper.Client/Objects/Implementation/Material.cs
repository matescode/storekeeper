using CommonBase;

using StoreKeeper.Client.Objects.DataProxy;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class Material : BaseObject<MaterialDataProxy>, IMaterial
    {
        public Material(MaterialDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IMaterial Implementation

        public ObjectId MaterialId
        {
            get { return Proxy.MaterialId; }
        }

        public string Code
        {
            get { return Proxy.Code; }
        }

        public string SupplierCode
        {
            get { return Proxy.SupplierCode; }
        }

        public ArticleType Type
        {
            get { return Proxy.Type; }
        }

        public string Name
        {
            get { return Proxy.Name; }
        }

        public double Price
        {
            get { return Proxy.Price; }
        }

        public double CurrentCount
        {
            get { return Proxy.CurrentCount; }
        }

        public double MissingInOrders
        {
            get
            {
                return Proxy.MissingInOrders;
            }
        }

        public double OrderedCount
        {
            get
            {
                return Proxy.OrderedCount;
            }
        }

        public int ProductCount
        {
            get
            {
                return Proxy.ProductCount;
            }
        }

        public double ExternStorageCount
        {
            get
            {
                return Proxy.ExternStorageCount;
            }
        }

        #endregion
    }
}