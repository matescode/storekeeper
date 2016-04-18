using CommonBase;
using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class MaterialOrder : BaseObject<MaterialOrderDataProxy>, IMaterialOrder
    {
        public MaterialOrder(MaterialOrderDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IMaterialOrder Implementation

        public ObjectId OrderId
        {
            get { return Proxy.OrderId; }
        }

        public ObjectId MaterialId
        {
            get { return Proxy.MaterialId; }
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

        public double OrderedCount
        {
            get { return Proxy.OrderedCount; }
        }

        public double CurrentTotalCount
        {
            get { return Proxy.CurrentTotalCount; }
        }

        public IMaterialOrderStatus MaterialOrderStatus
        {
            get { return Proxy.MaterialOrderStatus; }
        }

        #endregion
    }
}