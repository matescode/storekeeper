using System;
using StoreKeeper.Common.DataContracts.Accounting;

namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class ProductArticleReservation : UserObject
    {
        public Guid ProductArticleOrderId { get; set; }

        public Guid ProductArticleItemId { get; set; }

        public double CurrentCount { get; set; }

        public double ReservationCount { get; set; }

        public double OrderCount { get; set; }

        public virtual ProductArticleOrder ProductArticleOrder { get; set; }

        public virtual ProductArticleItem ProductArticleItem { get; set; }
    }
}