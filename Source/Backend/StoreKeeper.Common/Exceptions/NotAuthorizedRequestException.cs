using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Common.Exceptions
{
    [Serializable]
    public class NotAuthorizedRequestException : CommonException
    {
        public NotAuthorizedRequestException(Type type, SessionId sessionId)
            : base(type, LogId.NotAuthorizedRequest, "Not authorized request from the session '{0}' has been occured.", sessionId)
        {
        }

        protected NotAuthorizedRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}