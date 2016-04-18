using CommonBase;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Common.DataContracts.Sync
{
    public class ArticleData : ObjectBase
    {
        public ArticleData()
        {
            Id = ObjectId.NewId();
        }

        public int ExternalId { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public string Code { get; set; }

        public string SpecialCode { get; set; }

        public double OriginalCount { get; set; }

        public string InternalStorage { get; set; }

        public double PurchasingPrice { get; set; }

        public double SellingPrice { get; set; }

        public string OrderName { get; set; }

        public double OrderCount { get; set; }
    }
}