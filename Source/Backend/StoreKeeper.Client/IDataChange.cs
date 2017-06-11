using CommonBase;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.Client
{
    internal interface IDataChange
    {
        void GetLock();

        void RequestForCalculation();

        int CorrectPriority(int priority);

        void RefreshProductOrdersPriorities(int oldPriority, int newPriority, ObjectId changedOrderId);

        void ReloadMaterial(ObjectId materialId);

		AccountingOrderStatus GetOrderStatus(double orderCount, double accountingOrderCount);
    }
}