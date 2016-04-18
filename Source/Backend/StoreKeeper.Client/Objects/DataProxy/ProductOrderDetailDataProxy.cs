using System;
using CommonBase;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class ProductOrderDetailDataProxy : ProxyBase
    {
        public ProductOrderDetailDataProxy(IDataChange dataChange, ObjectId orderId)
            : base(dataChange)
        {
            OrderId = orderId;
        }

        #region Properties

        public ObjectId OrderId { get; set; }

        public ObjectId ProductId { get; set; }

        public string Name { get; set; }

        public ObjectId ItemId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = OrderId;
                ProductArticleOrder order = dataContext.ProductArticleOrders.Find(id);

                ProductId = order.ProductArticle.ArticleId;
                Name = order.ProductArticle.Article.Name;
                Title = order.ProductArticle.Article.Code;
                ItemId = ProductId;
                Description = Name;
            }
        }

        #endregion
    }
}