namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class Storage : ObjectBase
    {
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
    }
}