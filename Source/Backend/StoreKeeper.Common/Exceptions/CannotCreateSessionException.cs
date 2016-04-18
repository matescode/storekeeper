using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Common.Exceptions
{
    [Serializable]
    public class CannotCreateSessionException : CommonException
    {
        public CannotCreateSessionException(Type type)
            : base(type, LogId.CannotCreateSession, "Cannot create session.")
        {
        }

        protected CannotCreateSessionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}