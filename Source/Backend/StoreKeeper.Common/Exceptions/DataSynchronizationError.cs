using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;
using StoreKeeper.Common.DataContracts.Sync;

namespace StoreKeeper.Common.Exceptions
{
    [Serializable]
    public class DataSynchronizationError : CommonException
    {
        public DataSynchronizationError(Type type, DataSyncErrorType errorType)
            : base(type, LogId.AccountDataSyncFault, "Error during accounting data synchronization: {0}", errorType.ToString())
        {
        }

        public DataSynchronizationError(Type type, DataSyncErrorType errorType, string message)
            : base(type, LogId.AccountDataSyncFault, "Error during accounting data synchronization ({0}): {1}", errorType.ToString(), message)
        {
        }

        protected DataSynchronizationError(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}