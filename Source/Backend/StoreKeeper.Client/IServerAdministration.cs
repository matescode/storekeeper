using System;
using System.Collections.Generic;

using StoreKeeper.Client.Objects;

namespace StoreKeeper.Client
{
    public interface IServerAdministration
    {
        DateTime LastAccountingUpdate { get; }

        string ResponsibleUser { get; }

        string LockedBy { get; }

        List<IServerUser> Users { get; }

        IServerUser CreateUser(string username);

        bool RemoveUser(IServerUser user);
    }
}