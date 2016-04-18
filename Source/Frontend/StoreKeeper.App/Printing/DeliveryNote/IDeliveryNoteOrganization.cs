namespace StoreKeeper.App.Printing.DeliveryNote
{
    public interface IDeliveryNoteOrganization
    {
        string Company { get; }

        string Street { get; }

        string Number { get; }

        string ZipCode { get; }

        string City { get; }

        string CompanyId { get; }

        string TaxId { get; }
    }
}