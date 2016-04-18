using System;
using System.Linq;
using CommonBase;
using StoreKeeper.Client.Objects.Implementation;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class MaterialOrderDataProxy : ProxyBase
    {
        public MaterialOrderDataProxy(IDataChange dataChange, ObjectId materialId)
            : base(dataChange)
        {
            MaterialId = materialId;
        }

        #region Properties

        public ObjectId OrderId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public double CurrentCount { get; set; }

        public ObjectId MaterialId { get; set; }

        public double OrderedCount { get; set; }

        public IMaterialOrderStatus MaterialOrderStatus { get; set; }

        public double CurrentTotalCount { get; set; }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = MaterialId;
                ArticleOrder articleOrder = dataContext.ArticleOrders.FirstOrDefault(o => o.ArticleId == id);

                if (articleOrder == null)
                {
                    return;
                }

                OrderId = articleOrder.Id;
                Code = articleOrder.Article.Code;
                Name = articleOrder.Article.Name;
                CurrentCount = articleOrder.Article.ArticleStat.CurrentCount;
                OrderedCount = articleOrder.Count;
                CurrentTotalCount = articleOrder.Article.ArticleStat.CurrentCount + articleOrder.Article.ExternStorageCount;
                MaterialOrderStatus = new MaterialOrderStatus(DataChange, articleOrder.Count, articleOrder.Article.OrderCount);
            }
        }

        #endregion
    }
}