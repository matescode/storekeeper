using System;
using System.Runtime.Serialization;
using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class DatabaseLockedException : CommonException
    {
        public DatabaseLockedException(Type type, string message)
            : base(type, LogId.DatabaseLocked, message)
        {
        }

        private DatabaseLockedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}