using System;

namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class ProductOrderHistory
    {
        public Guid Id { get; set; }

        public Guid ProductArticleId { get; set; }

        public double Count { get; set; }

        public int Priority { get; set; }

        public double ProductionCount { get; set; }

        public DateTime? OrderPeriod { get; set; }

        public DateTime? PlannedPeriod { get; set; }

        public DateTime? EndPeriod { get; set; }

        public DateTime StampTime { get; set; }
    }
}