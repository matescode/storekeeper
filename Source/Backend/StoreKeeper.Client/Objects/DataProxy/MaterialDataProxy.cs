using System;
using System.Linq;

using CommonBase;

using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts;
using StoreKeeper.Common.DataContracts.Accounting;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class MaterialDataProxy : ProxyBase
    {
        public MaterialDataProxy(IDataChange dataChange, ObjectId materialId)
            : base(dataChange)
        {
            MaterialId = materialId;
        }

        #region Properties

        public ObjectId MaterialId { get; set; }

        public string Code { get; set; }

        public string SupplierCode { get; set; }

        public ArticleType Type { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double CurrentCount { get; set; }

        public double MissingInOrders { get; set; }

        public double OrderedCount { get; set; }

        public int ProductCount { get; set; }

        public double ExternStorageCount { get; set; }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = MaterialId;
                Article article = dataContext.Articles.Find(id);

                Code = article.Code;
                SupplierCode = article.OrderName;
                Type = article.ArticleType;
                Name = article.Name;
                Price = article.SellingPrice;
                CurrentCount = article.ArticleStat.CurrentCount;
                MissingInOrders = article.ArticleStat.MissingInOrders;
                OrderedCount = dataContext.GetOrderedCount(MaterialId, Type);
                ProductCount = article.ArticleStat.ProductCount;
                ExternStorageCount = article.ExternStorageCount;
            }
        }

        #endregion
    }
}