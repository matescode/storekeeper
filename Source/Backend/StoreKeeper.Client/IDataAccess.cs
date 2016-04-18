using System;
using System.Collections.Generic;
using CommonBase;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.Client
{
    public interface IDataAccess
    {
        IEnumerable<IMaterial> Materials { get; }

        IEnumerable<IMaterialOrder> MaterialOrders { get; }

        IEnumerable<IProductOrder> ProductOrders { get; }

        IEnumerable<IExternStorage> Storages { get; }

        IEnumerable<IExternStorageMaterial> ExternStorageMaterials { get; }

        IDeliveryNoteSettings DeliveryNoteSettings { get; }

        void ReloadData();

        void ReloadDataAsync(Action reloadAction = null);

        IProductOrder CreateProductOrder(string code, double count = 0);

        bool ResolveProductOrder(IProductOrder order, bool resolve);

        IProductStorageMapping GetProductStorageMapping(ObjectId productId);

        void SetStorageMappingAsync(ObjectId productId, ObjectId storageId, Action reloadAction);

        IProductOrderDetail GetProductOrderDetail(ObjectId productOrderId);

        IEnumerable<IProductOrderItem> GetProductOrderDetailItems(ObjectId productItemId, ObjectId productOrderId);
            
        IExternStorage CreateExternStorage(string name, string companyName, string street, string number, string zipCode, string city, string companyId, string taxId);

        bool RemoveExternStorage(IExternStorage externStorage);

        bool ExistsExternStorage(string name, ObjectId excludedStorageId);

        bool CanExternStorageBeSafelyRemoved(ObjectId storageId);

        IEnumerable<IArticleCode> GetMatchingArticleCodes(string codePart, ArticleCodeType codeType);

        bool IsCodeValid(string code, ArticleCodeType codeType);

        void RequestForCalculation();
    }
}