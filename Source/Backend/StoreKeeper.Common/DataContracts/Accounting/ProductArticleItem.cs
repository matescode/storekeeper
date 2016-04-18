using System;
using System.Collections.Generic;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Common.DataContracts.Accounting
{
    public class ProductArticleItem : ObjectBase
    {
        public Guid ProductArticleId { get; set; }

        public Guid ArticleId { get; set; }

        public double Quantity { get; set; }

        public Guid StorageId { get; set; }

        public bool SkipCalculation { get; set; }

        public virtual ProductArticle ProductArticle { get; set; }

        public virtual Article Article { get; set; }

        public virtual Storage Storage { get; set; }

        public virtual ICollection<ProductArticleReservation> ProductArticleReservations { get; set; }
    }
}