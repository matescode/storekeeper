using CommonBase;

namespace StoreKeeper.Client.Objects
{
    public interface IProductOrderDetail
    {
        ObjectId ItemId { get; }

        string Title { get; }

        string Description { get; }

        ObjectId OrderId { get; }

        ObjectId ProductId { get; }

        string Name { get; }
    }
}