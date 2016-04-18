using System;

using CommonBase;

using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts;
using StoreKeeper.Common.DataContracts.Accounting;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class ProductStorageMappingItemDataProxy : ProxyBase
    {
        private ObjectId _storageId;
        private bool _skipCalculation;

        public ProductStorageMappingItemDataProxy(IDataChange dataChange, ObjectId articleItemId)
            : base(dataChange)
        {
            ArticleItemId = articleItemId;
        }

        #region Properties

        public ObjectId ArticleItemId { get; set; }

        public ObjectId ArticleId { get; set; }

        public string Code { get; set; }

        public ArticleType Type { get; set; }

        public string Name { get; set; }

        public string Storage { get; set; }

        public ObjectId StorageId
        {
            get { return _storageId; }
            set
            {
                _storageId = value;
                ChangeValue(i => i.StorageId = _storageId);
            }
        }

        public bool SkipCalculation
        {
            get { return _skipCalculation; }
            set
            {
                _skipCalculation = value;
                ChangeValue(i => i.SkipCalculation = _skipCalculation);
            }
        } 

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = ArticleItemId;
                ProductArticleItem articleItem = dataContext.ProductArticleItems.Find(id);

                ArticleId = articleItem.ArticleId;
                Code = articleItem.Article.Code;
                Type = articleItem.Article.ArticleType;
                Name = articleItem.Article.Name;
                Storage = articleItem.Storage.Name;
                _storageId = articleItem.StorageId;
                _skipCalculation = articleItem.SkipCalculation;
            }
        }

        #endregion

        #region Internals and Helpers

        private void ChangeValue(Action<ProductArticleItem> changeAction)
        {
            DataChange.GetLock();

            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = ArticleItemId;
                ProductArticleItem articleItem = dataContext.ProductArticleItems.Find(id);

                changeAction(articleItem);

                dataContext.SaveChanges();
            }
        }

        #endregion
    }
}