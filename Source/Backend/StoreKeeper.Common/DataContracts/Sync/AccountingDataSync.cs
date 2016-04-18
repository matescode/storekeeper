using System;
using System.Collections.Generic;

namespace StoreKeeper.Common.DataContracts.Sync
{
    public class AccountingDataSync
    {
        public static AccountingDataSync EmptySync = new AccountingDataSync
        {
            Articles = new List<ArticleData>(),
            ArticleItems = new List<ArticleItemData>(),
            LastUpdate = DateTime.Now
        };

        public List<ArticleData> Articles { get; set; }

        public List<ArticleItemData> ArticleItems { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}