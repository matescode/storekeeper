using System;
using CommonBase;

namespace StoreKeeper.Client.Objects
{
    public interface IProductOrder : IOrder, ILoadable
    {
        ObjectId ProductId { get; }

        int Priority { get; set; }

        double OrderedCount { get; set; }

        double PossibleCount { get; }

        bool IsComplete { get; }

        DateTime? OrderPeriod { get; set; }

        DateTime? PlannedPeriod { get; set; }

        DateTime? EndPeriod { get; set; }
    }
}