using CommonBase;
using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ExternStorage : BaseObject<ExternStorageDataProxy>, IExternStorage
    {
        public ExternStorage(ExternStorageDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IExternStorage Implementation

        public ObjectId StorageId
        {
            get { return Proxy.StorageId; }
        }

        public string Name
        {
            get { return Proxy.Name; }
            set { Proxy.Name = value; }
        }

        public string Prefix
        {
            get { return Proxy.Prefix; }
            set { Proxy.Prefix = value; }
        }

        public bool IsExtern
        {
            get { return Proxy.IsExtern; }
        }

        public string CompanyName
        {
            get { return Proxy.CompanyName; }
            set { Proxy.CompanyName = value; }
        }

        public string Street
        {
            get { return Proxy.Street; }
            set { Proxy.Street = value; }
        }

        public string Number
        {
            get { return Proxy.Number; }
            set { Proxy.Number = value; }
        }

        public string ZipCode
        {
            get { return Proxy.ZipCode; }
            set { Proxy.ZipCode = value; }
        }

        public string City
        {
            get { return Proxy.City; }
            set { Proxy.City = value; }
        }

        public string CompanyId
        {
            get { return Proxy.CompanyId; }
            set { Proxy.CompanyId = value; }
        }

        public string TaxId
        {
            get { return Proxy.TaxId; }
            set { Proxy.TaxId = value; }
        }

        public void Save()
        {
            Proxy.Save();
        }

        #endregion
    }
}