using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using CommonBase;
using CommonBase.Log;

using StoreKeeper.Common.DataContracts;
using StoreKeeper.Common.DataContracts.Accounting;
using StoreKeeper.Common.DataContracts.StoreKeeper;
using StoreKeeper.Common.DataContracts.Sync;

namespace StoreKeeper.Common
{
    public class StoreKeeperDataContext : DbContext
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(StoreKeeperDataContext));

        private readonly Dictionary<int, Article> _articleCash;
        private readonly List<Article> _tmpArticles;
        private readonly List<ArticleStat> _tmpArticleStats;
        private readonly List<ProductArticle> _tmpProductArticles;
        private readonly List<ProductArticleItem> _tmpProductArticleItems;

        public StoreKeeperDataContext()
            : base(ConnectionStringHolder.Value)
        {
            Database.SetInitializer<StoreKeeperDataContext>(null);
            
            _articleCash = new Dictionary<int, Article>();
            _tmpArticles = new List<Article>();
            _tmpArticleStats = new List<ArticleStat>();
            _tmpProductArticles = new List<ProductArticle>();
            _tmpProductArticleItems = new List<ProductArticleItem>();
        }

        #region Entities

        public DbSet<Article> Articles { get; set; }

        public DbSet<ProductArticle> ProductArticles { get; set; }

        public DbSet<ProductArticleItem> ProductArticleItems { get; set; }

        public DbSet<SystemInformation> SystemInformations { get; set; }

        public DbSet<ArticleOrder> ArticleOrders { get; set; }

        public DbSet<ArticleStat> ArticleStats { get; set; }

        public DbSet<ProductArticleOrder> ProductArticleOrders { get; set; }

        public DbSet<ProductArticleReservation> ProductArticleReservations { get; set; }

        public DbSet<Storage> Storages { get; set; }

        public DbSet<ProductOrderCompletion> ProductOrderCompletions { get; set; }

        public DbSet<ArticleData> ArticleDatas { get; set; }

        public DbSet<ArticleItemData> ArticleItemDatas { get; set; }

        #endregion

        #region Properties

        public DateTime LastUpdate
        {
            get
            {
                DateTime result;
                SystemInformation lastUpdateInfo = GetSystemInformation(Constants.LastUpdateSystemValue);
                if (lastUpdateInfo == null)
                {
                    lastUpdateInfo = SystemInformations.Create();
                    lastUpdateInfo.Id = ObjectId.NewId();
                    lastUpdateInfo.Name = Constants.LastUpdateSystemValue;
                    result = new DateTime(1970, 1, 1);
                    lastUpdateInfo.Value = result.ToString();
                    SystemInformations.Add(lastUpdateInfo);
                    SaveChanges();
                }
                else
                {
                    result = Convert.ToDateTime(lastUpdateInfo.Value);
                }
                return result;
            }
            set
            {
                SystemInformation lastUpdateInfo = GetSystemInformation(Constants.LastUpdateSystemValue);
                if (lastUpdateInfo == null)
                {
                    lastUpdateInfo = SystemInformations.Create();
                    lastUpdateInfo.Id = ObjectId.NewId();
                    lastUpdateInfo.Name = Constants.LastUpdateSystemValue;
                    SystemInformations.Add(lastUpdateInfo);
                }

                lastUpdateInfo.Value = value.ToString();
                SaveChanges();
            }
        }

        public string ResponsibleUser
        {
            get
            {
                SystemInformation responsibleUser = GetSystemInformation(Constants.ResponsibleUser);
                return responsibleUser != null ? responsibleUser.Value : String.Empty;
            }
            set
            {
                SystemInformation responsibleUser = GetSystemInformation(Constants.ResponsibleUser);
                if (responsibleUser == null)
                {
                    responsibleUser = SystemInformations.Create();
                    responsibleUser.Id = ObjectId.NewId();
                    responsibleUser.Name = Constants.ResponsibleUser;
                    SystemInformations.Add(responsibleUser);
                }
                responsibleUser.Value = value;
                SaveChanges();
            }
        }

        public string LockedBy
        {
            get
            {
                DbRawSqlQuery<string> result = Database.SqlQuery<string>("SELECT Value from SystemInformations where Name='LockedBy'");
                return result.ToList()[0];
            }
        }

        public string LockedByUser
        {
            get
            {
                string userId = LockedBy;
                if (String.IsNullOrEmpty(userId))
                {
                    return "--";
                }

                DbRawSqlQuery<string> result = Database.SqlQuery<string>(String.Format("SELECT Name from Users where Id = '{0}'", userId));
                return result.ToList()[0];
            }
        }

        #endregion

        #region Public Methods

        public void TestConnect()
        {
            Database.ExecuteSqlCommand("SELECT 1");
        }

        public double GetOrderedCount(ObjectId articleId, ArticleType articleType)
        {
            Guid id = articleId.ToGuid();
            if (articleType == ArticleType.Card)
            {
                return Enumerable.Sum(ArticleOrders.Where(ao => ao.ArticleId == id), item => item.Count);
            }

            return Enumerable.Sum(ProductArticleOrders.Where(pao => pao.ProductArticle.ArticleId == id), item => item.Count);
        }

        public string GetSystemValue(string name)
        {
            SystemInformation systemInfo = GetSystemInformation(name);
            if (systemInfo != null)
            {
                return systemInfo.Value;
            }
            return null;
        }

        public void SaveLoadedData(AccountingDataSync dataSync)
        {
            using (DbContextTransaction trans = Database.BeginTransaction())
            {
                try
                {
                    Database.ExecuteSqlCommand("exec PrepareLoading");

                    ArticleDatas.AddRange(dataSync.Articles);
                    ArticleItemDatas.AddRange(dataSync.ArticleItems);
                    SaveChanges();

                    LastUpdate = dataSync.LastUpdate;

                    DbCommand command = Database.Connection.CreateCommand();
                    command.Transaction = trans.UnderlyingTransaction;
                    command.CommandTimeout = 0;
                    command.CommandText = "exec PostLoadUpdate";
                    command.ExecuteNonQuery();

                    trans.Commit();

                    Logger.Info("Data loaded");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    trans.Rollback();
                }
            }
        }

        public string GetStringConstant(string name)
        {
            SystemInformation info = GetSystemInformation(name);
            return info != null ? info.Value : String.Empty;
        }

        public void SetStringConstant(string name, string value)
        {
            SystemInformation info = GetSystemInformation(name);
            if (info == null)
            {
                info = new SystemInformation { Id = ObjectId.NewId(), Name = name };
                SystemInformations.Add(info);
            }
            info.Value = value;
        }

        public bool IsOrderValid(ObjectId orderId)
        {
            Guid id = orderId.ToGuid();
            ProductOrderCompletion orderCompletion = ProductOrderCompletions.SingleOrDefault(oc => oc.ProductArticleOrderId == id);
            return orderCompletion == null;
        }

        #endregion

        #region Internals and Helpers

        private SystemInformation GetSystemInformation(string name)
        {
            return SystemInformations.SingleOrDefault(s => s.Name == name);
        }

        #endregion
    }
}