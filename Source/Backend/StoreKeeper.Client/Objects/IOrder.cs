using CommonBase;

namespace StoreKeeper.Client.Objects
{
    public interface IOrder : IClientObject
    {
        ObjectId OrderId { get; }

        string Code { get; }

        string Name { get; }
        
        double CurrentCount { get; }
    }
}