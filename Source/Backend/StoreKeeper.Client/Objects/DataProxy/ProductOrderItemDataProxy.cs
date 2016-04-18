using System;
using System.Linq;

using CommonBase;

using StoreKeeper.Client.Objects.Implementation;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts;
using StoreKeeper.Common.DataContracts.Accounting;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class ProductOrderItemDataProxy : ProxyBase
    {
        public ProductOrderItemDataProxy(IDataChange dataChange, ObjectId itemId, ObjectId orderId)
            : base(dataChange)
        {
            ItemId = itemId;
            OrderId = orderId;
        }

        #region Properties

        public ObjectId ItemId { get; set; }

        public ObjectId OrderId { get; set; }

        public ObjectId ArticleId { get; set; }

        public string Code { get; set; }

        public ArticleType Type { get; set; }

        public string Name { get; set; }

        public double Count { get; set; }

        public double StockAvailable { get; set; }

        public string Storage { get; set; }

        public double ProductionReservation { get; set; }

        public double OrderCount { get; set; }

        public IMaterialOrderStatus MaterialOrderStatus { get; set; }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = ItemId;
                Guid ordId = OrderId;
                ProductArticleItem articleItem = dataContext.ProductArticleItems.Find(id);
                ProductArticleReservation reservation = articleItem.ProductArticleReservations.FirstOrDefault(r => r.ProductArticleOrderId == ordId);

                ArticleId = articleItem.ArticleId;
                Code = articleItem.Article.Code;
                Type = articleItem.Article.ArticleType;
                Name = articleItem.Article.Name;
                Count = articleItem.Quantity;
                StockAvailable = reservation != null ? reservation.CurrentCount : 0;
                Storage = articleItem.Storage.Name;
                ProductionReservation = reservation != null ? reservation.ReservationCount : 0;
                OrderCount = reservation != null ? reservation.OrderCount : 0;

                if (Type == ArticleType.Card)
                {
                    ArticleOrder articleOrder = dataContext.ArticleOrders.FirstOrDefault(ao => ao.ArticleId == articleItem.ArticleId);
                    if (reservation != null && Math.Abs(reservation.OrderCount - 0) < 0.001)
                    {
                        articleOrder = null;
                    }

                    MaterialOrderStatus = new MaterialOrderStatus(DataChange, articleOrder != null ? articleOrder.Count : 0, articleOrder != null ? articleOrder.Article.OrderCount : 0);
                }
                else
                {
                    MaterialOrderStatus = new MaterialOrderStatus(DataChange, 0, 0);
                }
            }
        }

        #endregion
    }
}