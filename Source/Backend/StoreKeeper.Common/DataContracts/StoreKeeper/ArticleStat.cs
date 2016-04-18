using System;
using StoreKeeper.Common.DataContracts.Accounting;

namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class ArticleStat : ObjectBase
    {
        public Guid ArticleId { get; set; }

        public Guid StorageId { get; set; }

        public double CurrentCount { get; set; }

        public double MissingInOrders { get; set; }

        public int ProductCount { get; set; }

        public virtual Article Article { get; set; }

        public virtual Storage Storage { get; set; }
    }
}