using CommonBase;

namespace StoreKeeper.Client.Objects
{
    public interface IMaterialOrder : IOrder
    {
        ObjectId MaterialId { get; }

        double OrderedCount { get; }

        IMaterialOrderStatus MaterialOrderStatus { get; }

        double CurrentTotalCount { get; }
    }
}