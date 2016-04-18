using System.Collections.Generic;

using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.Printing.DeliveryNote
{
    public interface IDeliveryNoteDataSource
    {
        List<IDeliveryNoteItem> Items { get; }

        IDeliveryNoteOrganization Supplier { get; }

        IDeliveryNoteOrganization Subscriber { get; }

        IDeliveryNoteSettings Settings { get; }
    }
}