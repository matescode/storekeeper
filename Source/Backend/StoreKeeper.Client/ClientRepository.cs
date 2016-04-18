using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using CommonBase;
using CommonBase.Log;

using StoreKeeper.Client.Exceptions;
using StoreKeeper.Client.Objects;
using StoreKeeper.Client.Objects.DataProxy;
using StoreKeeper.Client.Objects.Implementation;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts;
using StoreKeeper.Common.DataContracts.Accounting;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client
{
    internal class ClientRepository : IClientRepository, IDataAccess, IDataChange
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(ClientRepository));
        private ILongOperationHandler _longOperationHandler;
        private readonly List<IMaterial> _materials;
        private readonly List<IMaterialOrder> _materialOrders;
        private readonly List<IProductOrder> _productOrders;
        private readonly List<IExternStorage> _externStorages;
        private readonly List<IExternStorageMaterial> _externStorageMaterials;
        private readonly IDatabaseAccess _databaseAccess;
        private IDeliveryNoteSettings _deliveryNoteSettings;

        public ClientRepository(IDatabaseAccess databaseAccess)
        {
            _databaseAccess = databaseAccess;
            _materials = new List<IMaterial>();
            _materialOrders = new List<IMaterialOrder>();
            _productOrders = new List<IProductOrder>();
            _externStorages = new List<IExternStorage>();
            _externStorageMaterials = new List<IExternStorageMaterial>();
            DatabaseStatus = DatabaseStatus.NotConnected;
        }

        #region IClientRepository Implementation

        public DatabaseStatus DatabaseStatus { get; private set; }

        public IDataAccess DataAccess
        {
            get { return this; }
        }

        public ILongOperationHandler LongOperationHandler
        {
            private get { return _longOperationHandler; }
            set { _longOperationHandler = value; }
        }

        public IClientMessenger Messenger
        {
            private get;
            set;
        }

        public void Open()
        {
            try
            {
                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    dataContext.TestConnect();
                }
                DatabaseStatus = DatabaseStatus.Connected;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                DatabaseStatus = DatabaseStatus.NotConnected;
            }
        }

        public void CheckDatabaseStatus()
        {
            string lockedBy = String.Empty;
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                lockedBy = dataContext.LockedBy;
            }

            if (!String.IsNullOrWhiteSpace(lockedBy))
            {
                DatabaseStatus = String.Compare(lockedBy, UserContext.UserId, StringComparison.Ordinal) == 0 ? DatabaseStatus.Locked : DatabaseStatus.Blocked;
            }
        }

        public void ResetDatabaseStatus()
        {
            DatabaseStatus = DatabaseStatus.Connected;
            CheckDatabaseStatus();
            Messenger.DatabaseStatusChanged();
        }

        #endregion

        #region IDataAccess Implementation

        public IEnumerable<IMaterial> Materials
        {
            get
            {
                LoadMaterials();
                return _materials;
            }
        }

        public IEnumerable<IMaterialOrder> MaterialOrders
        {
            get
            {
                LoadMaterialOrders();
                return _materialOrders;
            }
        }

        public IEnumerable<IProductOrder> ProductOrders
        {
            get
            {
                LoadProductOrders();
                return _productOrders;
            }
        }

        public IEnumerable<IExternStorage> Storages
        {
            get
            {
                LoadExternStorages();
                return _externStorages;
            }
        }

        public IEnumerable<IExternStorageMaterial> ExternStorageMaterials
        {
            get
            {
                LoadExternStorageMaterials();
                return _externStorageMaterials;
            }
        }

        public IDeliveryNoteSettings DeliveryNoteSettings
        {
            get { return _deliveryNoteSettings ?? (_deliveryNoteSettings = new DeliveryNoteSettings(new DeliveryNoteSettingsDataProxy(this))); }
        }

        public void ReloadData()
        {
            _materials.Clear();
            _materialOrders.Clear();
            _productOrders.Clear();
            _externStorages.Clear();
            _externStorageMaterials.Clear();

            LoadMaterials();
            LoadProductOrders();
            LoadMaterialOrders();
            LoadExternStorages();
            LoadExternStorageMaterials();

            GC.Collect();

            if (Messenger != null)
            {
                Messenger.DataUpdated = true;
            }
        }

        public void ReloadDataAsync(Action reloadAction = null)
        {
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        _longOperationHandler.Start("DataLoading");
                        ReloadData();
                        _longOperationHandler.End(reloadAction);
                    }
                    catch (Exception ex)
                    {
                        _longOperationHandler.OperationFailed(ex.Message);
                    }
                });
        }

        public IProductOrder CreateProductOrder(string code, double count = 0)
        {
            GetLock();
            ProductArticleOrder order;
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Article article = dataContext.Articles.FirstOrDefault(a => a.Code.ToUpper() == code.ToUpper());
                if (article == null || (article.ArticleType != ArticleType.Product))
                {
                    throw new ProductArticleNotFoundException(GetType(), code);
                }

                ProductArticle productArticle =
                    dataContext.ProductArticles.FirstOrDefault(pa => pa.ArticleId == article.Id);
                if (productArticle == null)
                {
                    throw new ProductArticleNotFoundException(GetType(), code);
                }

                order = new ProductArticleOrder();
                order.Id = ObjectId.NewId();
                order.ProductArticleId = productArticle.Id;
                order.Count = count;
                order.Priority = dataContext.ProductArticleOrders.Count() + 1;
                order.UserId = UserContext.UserId;

                dataContext.ProductArticleOrders.Add(order);
                dataContext.SaveChanges();
            }

            IProductOrder productOrder = new ProductOrder(new ProductOrderDataProxy(this, order.Id));
            _productOrders.Add(productOrder);
            RequestForCalculation();
            return productOrder;
        }

        public bool ResolveProductOrder(IProductOrder order, bool resolve)
        {
            try
            {
                GetLock();

                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    SqlParameter orderIdParam = new SqlParameter("@OrderId", SqlDbType.UniqueIdentifier) { Value = order.OrderId.ToGuid() };
                    SqlParameter resolveParam = new SqlParameter("@Resolve", SqlDbType.Bit) { Value = resolve };
                    dataContext.Database.ExecuteSqlCommand("exec ResolveProductOrder @OrderId, @Resolve", orderIdParam, resolveParam);
                }

                _productOrders.Clear();
                LoadProductOrders();
                RequestForCalculation();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public IProductStorageMapping GetProductStorageMapping(ObjectId productId)
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid articleId = productId;
                ProductArticle productArticle = dataContext.ProductArticles.FirstOrDefault(pa => pa.ArticleId == articleId);
                if (productArticle == null)
                {
                    throw new ProductArticleNotFoundException(GetType(), productId);
                }
                return new ProductStorageMapping(new ProductStorageMappingDataProxy(this, productArticle.Id));
            }
        }

        public void SetStorageMappingAsync(ObjectId productId, ObjectId storageId, Action reloadAction)
        {
            Task.Factory.StartNew(() =>
                {
                    GetLock();

                    _longOperationHandler.Start("SettingStorageOperation");
                    Guid articleId = productId;

                    using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                    {
                        ProductArticle productArticle = dataContext.ProductArticles.FirstOrDefault(pa => pa.ArticleId == articleId);
                        if (productArticle == null)
                        {
                            throw new ProductArticleNotFoundException(GetType(), productId);
                        }
                        foreach (
                            ProductArticleItem item in
                                productArticle.ProductArticleItems.Where(
                                    i => i.Article.Type == (int)ArticleType.Card && !i.SkipCalculation))
                        {
                            item.StorageId = storageId;
                        }
                        dataContext.SaveChanges();
                    }

                    _longOperationHandler.End(new LongOperationResult
                        {
                            CustomAction = reloadAction
                        });
                });
        }

        public IProductOrderDetail GetProductOrderDetail(ObjectId productOrderId)
        {
            return new ProductOrderDetail(new ProductOrderDetailDataProxy(this, productOrderId));
        }

        public IEnumerable<IProductOrderItem> GetProductOrderDetailItems(ObjectId productItemId, ObjectId productOrderId)
        {
            List<IProductOrderItem> items = new List<IProductOrderItem>();
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                ProductArticle productArticle = dataContext.ProductArticles.First(pa => pa.ArticleId == (Guid)productItemId);
                foreach (ProductArticleItem item in productArticle.ProductArticleItems.Where(i => !i.SkipCalculation).OrderBy(i => i.Article.Code))
                {
                    items.Add(new ProductOrderItem(new ProductOrderItemDataProxy(this, item.Id, productOrderId)));
                }
            }

            return items;
        }

        public IExternStorage CreateExternStorage(string name, string companyName, string street, string number, string zipCode, string city, string companyId, string taxId)
        {
            GetLock();
            Storage storage;
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                storage = new Storage();
                storage.Id = ObjectId.NewId();
                storage.Name = name;
                storage.IsExtern = true;
                storage.CompanyName = companyName;
                storage.Street = street ?? String.Empty;
                storage.Number = number;
                storage.ZipCode = zipCode;
                storage.City = city;
                storage.CompanyId = companyId ?? String.Empty;
                storage.TaxId = taxId ?? String.Empty;

                dataContext.Storages.Add(storage);
                dataContext.SaveChanges();
            }

            ExternStorage externStorage = new ExternStorage(new ExternStorageDataProxy(this, storage.Id));
            _externStorages.Add(externStorage);
            return externStorage;
        }

        public bool ExistsExternStorage(string name, ObjectId excludedStorageId)
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                return dataContext.Storages.FirstOrDefault(e => e.Name.ToUpper().Equals(name.ToUpper()) && e.Id != (Guid)excludedStorageId) != null;
            }
        }

        public bool CanExternStorageBeSafelyRemoved(ObjectId storageId)
        {
            Guid id = storageId;
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                return !dataContext.ProductArticleItems.Any(m => m.StorageId == id)
                       && !dataContext.ArticleStats.Any(es => es.StorageId == id);
            }
        }

        public bool RemoveExternStorage(IExternStorage externStorage)
        {
            Guid storageId = externStorage.StorageId;
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Storage storage = dataContext.Storages.FirstOrDefault(es => es.Id == storageId);
                if (storage != null)
                {
                    dataContext.Storages.Remove(storage);
                    _externStorages.Remove(externStorage);
                    dataContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<IArticleCode> GetMatchingArticleCodes(string codePart, ArticleCodeType codeType)
        {
            int codeTypeValue = (int)codeType;
            List<IArticleCode> result = new List<IArticleCode>();
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                foreach (Article article in dataContext.Articles.Where(a => a.Code.ToUpper().StartsWith(codePart.ToUpper())))
                {
                    if (article.Type == codeTypeValue || codeTypeValue == 0)
                    {
                        result.Add(new ArticleCode(article.Code, article.Name));
                    }
                }
            }
            return result;
        }

        public bool IsCodeValid(string code, ArticleCodeType codeType)
        {
            if (DatabaseStatus == DatabaseStatus.Blocked)
            {
                return false;
            }

            int codeTypeValue = (int)codeType;
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Article article = dataContext.Articles.FirstOrDefault(a => a.Code.ToUpper() == code.ToUpper());
                if (article == null)
                {
                    return false;
                }
                return article.Type == codeTypeValue || codeTypeValue == 0;
            }
        }

        #endregion

        #region IDataChange Implementation

        public void GetLock()
        {
            _databaseAccess.GetLock();
        }

        public void RequestForCalculation()
        {
            Messenger.CalculationRequested = true;
        }

        public int CorrectPriority(int priority)
        {
            if (priority <= 0)
            {
                return 1;
            }

            int count = ProductOrders.Count();
            return priority > count ? count : priority;
        }

        public void RefreshProductOrdersPriorities(int oldPriority, int newPriority, ObjectId changedOrderId)
        {
            int value = 1;
            if (newPriority > oldPriority)
            {
                value = -1;
            }

            int min = Math.Min(oldPriority, newPriority);
            int max = Math.Max(oldPriority, newPriority);

            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                foreach (ProductArticleOrder order in dataContext.ProductArticleOrders.Where(o => o.Priority >= min && o.Priority <= max))
                {
                    if (order.Id != (Guid)changedOrderId)
                    {
                        order.Priority += value;
                    }
                }

                dataContext.SaveChanges();
            }
        }

        public void ReloadMaterial(ObjectId materialId)
        {
            IMaterial material = _materials.First(m => m.MaterialId == materialId);
            material.Load();
        }

        public AccountingOrderStatus GetOrderStatus(double orderCount, double accountingOrderCount)
        {
            if (orderCount > 0)
            {
                if (accountingOrderCount > 0)
                {
                    if (accountingOrderCount >= orderCount)
                    {
                        return AccountingOrderStatus.OrderedCompletely;
                    }
                    return AccountingOrderStatus.Ordered;
                }
                return AccountingOrderStatus.NotOrdered;
            }
            return AccountingOrderStatus.NotNecessary;
        }

        #endregion

        #region Internals and Helpers

        private void LoadMaterials()
        {
            if (_materials.Count > 0)
            {
                return;
            }

            try
            {
                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    foreach (Article article in dataContext.Articles.OrderBy(a => a.Code))
                    {
                        if (article.ArticleStat != null)
                        {
                            _materials.Add(new Material(new MaterialDataProxy(this, article.Id)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void LoadMaterialOrders()
        {
            if (_materialOrders.Count > 0)
            {
                return;
            }

            List<Guid> existing = new List<Guid>();
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                foreach (ArticleOrder articleOrder in dataContext.ArticleOrders.ToList().OrderBy(o => o.Article.Code).ThenBy(o => o.UserPriority))
                {
                    if (existing.Contains(articleOrder.ArticleId))
                    {
                        continue;
                    }

                    _materialOrders.Add(new MaterialOrder(new MaterialOrderDataProxy(this, articleOrder.ArticleId)));
                    existing.Add(articleOrder.ArticleId);
                }
            }
        }

        private void LoadProductOrders()
        {
            if (_productOrders.Count > 0)
            {
                return;
            }

            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                foreach (ProductArticleOrder productOrder in dataContext.ProductArticleOrders.OrderBy(po => po.Priority))
                {
                    if (dataContext.IsOrderValid(productOrder.Id))
                    {
                        _productOrders.Add(new ProductOrder(new ProductOrderDataProxy(this, productOrder.Id)));
                    }
                }
            }
        }

        private void LoadExternStorages()
        {
            if (_externStorages.Count > 0)
            {
                return;
            }

            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                foreach (Storage storage in dataContext.Storages)
                {
                    _externStorages.Add(new ExternStorage(new ExternStorageDataProxy(this, storage.Id)));
                }
            }
        }

        private void LoadExternStorageMaterials()
        {
            if (_externStorageMaterials.Count > 0)
            {
                return;
            }

            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                foreach (ArticleStat externStorageStat in dataContext.ArticleStats.OrderBy(es => es.Article.Code))
                {
                    if (externStorageStat.StorageId != Constants.CentralStorageId)
                    {
                        _externStorageMaterials.Add(new ExternStorageMaterial(new ExternStorageMaterialDataProxy(this, externStorageStat.ArticleId, externStorageStat.StorageId)));
                    }
                }
            }
        }

        private void ReloadExternStorageStat(ObjectId statId)
        {
            IExternStorageMaterial obj = ExternStorageMaterials.FirstOrDefault(e => e.StatId == statId);
            if (obj == null)
            {
                return;
            }

            obj.Load();

            IMaterial material = _materials.First(m => m.MaterialId == obj.MaterialId);
            material.Load();
        }

        #endregion
    }
}