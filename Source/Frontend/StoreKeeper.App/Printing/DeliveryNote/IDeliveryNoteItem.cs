namespace StoreKeeper.App.Printing.DeliveryNote
{
    public interface IDeliveryNoteItem
    {
        string SpecialCode { get; }

        string CentralCode { get; }

        string Name { get; }

        double AmountValue { get; }

        string Amount { get; }
    }
}