using System;
using CommonBase;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Common.DataContracts.Sync
{
    public class ArticleItemData : ObjectBase
    {
        public ArticleItemData()
        {
            Id = ObjectId.NewId();
        }

        public Guid ArticleDataId { get; set; }

        public int ExternalId { get; set; }

        public string Code { get; set; }

        public string InternalStorage { get; set; }

        public double Quantity { get; set; }

        public bool Updated { get; set; }
    }
}