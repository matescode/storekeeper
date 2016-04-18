using System;
using System.Threading.Tasks;

namespace StoreKeeper.Client
{
    public interface ILongOperationHandler
    {
        TaskScheduler TaskScheduler { get; }

        void Start(string operationName = null, string firstOperationPart = null);

        void Next(string operationPart);

        void End(ILongOperationResult result);

        void End(Action finalAction = null);

        void OperationFailed(string reason, bool localize = false);

        void Refresh(ILongOperationResult result);
    }
}