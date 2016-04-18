using System;
using System.Collections.Generic;
using StoreKeeper.Common.DataContracts.Accounting;

namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class ProductArticleOrder : UserObject
    {
        public Guid ProductArticleId { get; set; }

        public double Count { get; set; }

        public int Priority { get; set; }

        public double ProductionCount { get; set; }

        public DateTime? OrderPeriod { get; set; }

        public DateTime? PlannedPeriod { get; set; }

        public DateTime? EndPeriod { get; set; }

        public virtual ProductArticle ProductArticle { get; set; }

        public virtual ICollection<ProductArticleReservation> ProductArticleReservations { get; set; }
    }
}