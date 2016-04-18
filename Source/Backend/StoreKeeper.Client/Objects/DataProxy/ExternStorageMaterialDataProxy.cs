using System;
using System.Linq;

using CommonBase;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class ExternStorageMaterialDataProxy : ProxyBase
    {
        public ExternStorageMaterialDataProxy(IDataChange dataChange, ObjectId materialId, ObjectId storageId)
            : base(dataChange)
        {
            MaterialId = materialId;
            StorageId = storageId;
        }

        #region Properties

        public ObjectId StatId { get; set; }

        public ObjectId MaterialId { get; set; }

        public ObjectId StorageId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public double CurrentCount { get; set; }

        public string Company { get; set; }

        public double CentralStorageCount { get; set; }

        public double MissingCount { get; set; }

        public string SpecialCode { get; set; }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid articleId = MaterialId;
                Guid storageId = StorageId;

                ArticleStat storageStat = dataContext.ArticleStats.FirstOrDefault(e => e.ArticleId == articleId && e.StorageId == storageId);

                if (storageStat == null)
                {
                    return;
                }

                StatId = storageStat.Id;
                Code = storageStat.Article.Code;
                Name = storageStat.Article.Name;
                CurrentCount = storageStat.CurrentCount;
                Company = storageStat.Storage.Name;
                CentralStorageCount = storageStat.Article.ArticleStat != null ? storageStat.Article.ArticleStat.CurrentCount : 0;
                MissingCount = storageStat.MissingInOrders;
                SpecialCode = storageStat.Article.SpecialCode;
            }
        }

        #endregion
    }
}