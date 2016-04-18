using System;
using System.Collections.Generic;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Common.DataContracts.Accounting
{
    public class ProductArticle
    {
        public Guid Id { get; set; }

        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; }

        public virtual ICollection<ProductArticleItem> ProductArticleItems { get; set; }

        public virtual ICollection<ProductArticleOrder> ProductArticleOrders { get; set; }
    }
}