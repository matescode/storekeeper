using CommonBase;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.Client.Objects
{
    public interface IProductOrderItem
    {
        ObjectId ItemId { get; }

        string Code { get; }

        ArticleType Type { get; }

        string Name { get; }

        double Count { get; }

        double StockAvailable { get; }

        string Storage { get; }

        double ProductionReservation { get; }

        double OrderCount { get; }

        IMaterialOrderStatus MaterialOrderStatus { get; }
    }
}