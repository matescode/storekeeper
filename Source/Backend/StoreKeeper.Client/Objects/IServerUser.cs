using System;

namespace StoreKeeper.Client.Objects
{
    public interface IServerUser : IClientObject
    {
        Guid UserId { get; }

        string Name { get; set; }

        string SecurityToken { get; }
    }
}