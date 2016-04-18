using System;
using StoreKeeper.Common.DataContracts.Accounting;

namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class ArticleOrder : UserObject
    {
        public Guid ArticleId { get; set; }

        public double Count { get; set; }

        public virtual Article Article { get; set; }
    }
}