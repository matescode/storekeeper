using System;

using CommonBase;

using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.StoreKeeper;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class ExternStorageDataProxy : ProxyBase
    {
        public ExternStorageDataProxy(IDataChange dataChange, ObjectId storageId)
            : base(dataChange)
        {
            StorageId = storageId;
        }

        #region Properties

        public ObjectId StorageId { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public bool IsExtern { get; set; }

        public string CompanyName { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string CompanyId { get; set; }

        public string TaxId { get; set; }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = StorageId;
                Storage storage = dataContext.Storages.Find(id);

                Name = storage.Name;
                Prefix = storage.Prefix;
                IsExtern = storage.IsExtern;
                CompanyName = storage.CompanyName;
                Street = storage.Street;
                Number = storage.Number;
                ZipCode = storage.ZipCode;
                City = storage.City;
                CompanyId = storage.CompanyId;
                TaxId = storage.TaxId;
            }
        }

        #endregion

        #region Internals and Helpers

        internal void Save()
        {
            DataChange.GetLock();

            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Guid id = StorageId;
                Storage storage = dataContext.Storages.Find(id);

                storage.Name = Name;
                storage.Prefix = id != Constants.CentralStorageId ? Prefix : null;
                storage.IsExtern = IsExtern;
                storage.CompanyName = CompanyName;
                storage.Street = Street;
                storage.Number = Number;
                storage.ZipCode = ZipCode;
                storage.City = City;
                storage.CompanyId = CompanyId;
                storage.TaxId = TaxId;

                dataContext.SaveChanges();
            }
        }

        #endregion
    }
}