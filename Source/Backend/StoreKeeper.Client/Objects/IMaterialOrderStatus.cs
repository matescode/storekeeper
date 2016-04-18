namespace StoreKeeper.Client.Objects
{
    public interface IMaterialOrderStatus
    {
        AccountingOrderStatus AccountingOrderStatus { get; }

        double AccountingOrderCount { get; }
    }
}