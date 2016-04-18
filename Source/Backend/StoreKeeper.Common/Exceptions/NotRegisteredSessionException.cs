using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Common.Exceptions
{
    [Serializable]
    public class NotRegisteredSessionException : CommonException
    {
        public NotRegisteredSessionException(Type type, SessionId sessionId)
            : base(type, LogId.SessionNotRegistered, string.Format("Session '{0}' is not registered", sessionId))
        {
        }

        protected NotRegisteredSessionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}