using System;

using CommonBase;
using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ProductOrder : BaseObject<ProductOrderDataProxy>, IProductOrder
    {
        public ProductOrder(ProductOrderDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IProductOrder Implementation

        public ObjectId OrderId
        {
            get { return Proxy.OrderId; }
        }

        public ObjectId ProductId
        {
            get { return Proxy.ProductId; }
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
            set { Proxy.OrderedCount = value; }
        }

        public DateTime? OrderPeriod
        {
            get { return Proxy.OrderPeriod; }
            set { Proxy.OrderPeriod = value; }
        }

        public DateTime? PlannedPeriod
        {
            get { return Proxy.PlannedPeriod; }
            set { Proxy.PlannedPeriod = value; }
        }

        public DateTime? EndPeriod
        {
            get { return Proxy.EndPeriod; }
            set { Proxy.EndPeriod = value; }
        }

        public int Priority
        {
            get { return Proxy.Priority; }
            set { Proxy.Priority = value; }
        }

        public double PossibleCount
        {
            get { return Proxy.PossibleCount; }
        }

        public bool IsComplete
        {
            get { return PossibleCount >= OrderedCount && OrderedCount > 0; }
        }

        #endregion
    }
}