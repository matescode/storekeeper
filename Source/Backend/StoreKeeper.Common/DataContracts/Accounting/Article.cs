using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Common.DataContracts.Accounting
{
    public class Article : ObjectBase
    {
        public string Name { get; set; }

        public int Type { get; set; }

        public string Code { get; set; }

        public string SpecialCode { get; set; }

        public double ExternStorageCount { get; set; }

        public double PurchasingPrice { get; set; }

        public double SellingPrice { get; set; }

        public string OrderName { get; set; }

        public double OrderCount { get; set; }

        public bool Updated { get; set; }

        public virtual ICollection<ArticleOrder> ArticleOrders { get; set; }

        public virtual ICollection<ArticleStat> ArticleStats { get; set; }

        [NotMapped]
        public ArticleType ArticleType
        {
            get { return (ArticleType)Type; }
            set { Type = (int)value; }
        }

        [NotMapped]
        public ArticleStat ArticleStat
        {
            get { return ArticleStats.FirstOrDefault(artStat => artStat.StorageId == Constants.CentralStorageId); }
        }
    }
}