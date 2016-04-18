using System;
using CommonBase;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class ProductOrderDataProxy : ProxyBase
    {
        private double _orderedCount;
        private int _priority;
        private DateTime? _orderPeriod;
        private DateTime? _plannedPeriod;
        private DateTime? _endPeriod;

        public ProductOrderDataProxy(IDataChange dataChange, ObjectId orderId)
            : base(dataChange)
        {
            OrderId = orderId;
        }

        #region Properties

        public ObjectId OrderId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public double CurrentCount { get; set; }

        public double OrderedCount
        {
            get { return _orderedCount; }
            set
            {
                _orderedCount = value;
                ChangeValue(o => o.Count = _orderedCount);
            }
        }

        public ObjectId ProductId { get; set; }

        public int Priority
        {
            get { return _priority; }
            set
            {
                int newValue = DataChange.CorrectPriority(value);
                int old = Priority;
                if (newValue == old)
                {
                    return;
                }

                _priority = value;
                ChangeValue(o => o.Priority = _priority);
                DataChange.RefreshProductOrdersPriorities(old, newValue, OrderId);
            }
        }

        public double PossibleCount { get; set; }

        public DateTime? OrderPeriod
        {
            get { return _orderPeriod; }
            set
            {
                _orderPeriod = value;
                ChangeValue(o => o.OrderPeriod = _orderPeriod);
            }
        }

        public DateTime? PlannedPeriod
        {
            get { return _plannedPeriod; }
            set
            {
                _plannedPeriod = value;
                ChangeValue(o => o.PlannedPeriod = _plannedPeriod);
            }
        }

        public DateTime? EndPeriod
        {
            get { return _endPeriod; }
            set
            {
                _endPeriod = value;
                ChangeValue(o => o.EndPeriod = _endPeriod);
            }
        }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = OrderId;
                ProductArticleOrder order = dataContext.ProductArticleOrders.Find(id);

                ProductId = order.ProductArticle.ArticleId;
                Code = order.ProductArticle.Article.Code;
                Name = order.ProductArticle.Article.Name;
                CurrentCount = order.ProductArticle.Article.ArticleStat.CurrentCount;
                _orderedCount = order.Count;
                _orderPeriod = order.OrderPeriod;
                _plannedPeriod = order.PlannedPeriod;
                _endPeriod = order.EndPeriod;
                _priority = order.Priority;
                PossibleCount = order.ProductionCount;
            }
        }

        #endregion

        #region Internals and Helpers

        private void ChangeValue(Action<ProductArticleOrder> changeAction)
        {
            DataChange.GetLock();

            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = OrderId;
                ProductArticleOrder order = dataContext.ProductArticleOrders.Find(id);

                changeAction(order);
                dataContext.SaveChanges();
            }
        }

        #endregion
    }
}