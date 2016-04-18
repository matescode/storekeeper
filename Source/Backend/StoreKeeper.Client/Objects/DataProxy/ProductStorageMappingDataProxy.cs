using System;
using System.Collections.Generic;
using CommonBase;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.Accounting;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class ProductStorageMappingDataProxy : ProxyBase
    {
        private readonly List<ProductStorageMappingItemDataProxy> _itemProxies;

        public ProductStorageMappingDataProxy(IDataChange dataChange, ObjectId productArticleId)
            : base(dataChange)
        {
            _itemProxies = new List<ProductStorageMappingItemDataProxy>();
            ProductArticleId = productArticleId;
        }

        #region Properties

        public ObjectId ProductArticleId { get; set; }

        public ObjectId ProductId { get; set; }

        public string Name { get; set; }

        public List<ProductStorageMappingItemDataProxy> ItemProxies
        {
            get { return _itemProxies; }
        }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = ProductArticleId;
                ProductArticle productArticle = dataContext.ProductArticles.Find(id);
                ProductId = productArticle.ArticleId;
                Name = productArticle.Article.Name;

                _itemProxies.Clear();
                foreach (ProductArticleItem item in productArticle.ProductArticleItems)
                {
                    _itemProxies.Add(new ProductStorageMappingItemDataProxy(DataChange, item.Id));
                }
            }
        }

        #endregion
    }
}