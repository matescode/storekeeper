using System;
using System.Collections.Generic;
using CommonBase;

namespace StoreKeeper.Client
{
    public interface ILongOperationResult
    {
        bool RefreshProductOrders { get; }

        bool RefreshExternStorageStats { get; }

        bool RefreshMaterialOrders { get; }

        bool RefreshAllMaterial { get; }

        bool RefreshAll { get; }

        IEnumerable<ObjectId> MaterialRefreshList { get; }

        Action CustomAction { get; }

        bool DataPublished { get; }
    }
}