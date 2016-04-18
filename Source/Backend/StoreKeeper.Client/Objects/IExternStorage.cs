using CommonBase;

namespace StoreKeeper.Client.Objects
{
    public interface IExternStorage : IClientObject
    {
        ObjectId StorageId { get; }

        string Name { get; set; }

        string Prefix { get; set; }

        bool IsExtern { get; }

        string CompanyName { get; set; }

        string Street { get; set; }

        string Number { get; set; }

        string ZipCode { get; set; }

        string City { get; set; }

        string CompanyId { get; set; }

        string TaxId { get; set; }

        void Save();
    }
}