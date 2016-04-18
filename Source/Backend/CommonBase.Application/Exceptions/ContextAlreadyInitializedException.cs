using System;
using System.Runtime.Serialization;
using CommonBase.Exceptions;

namespace CommonBase.Application.Exceptions
{
    [Serializable]
    public class ContextAlreadyInitializedException : CommonException
    {
        public ContextAlreadyInitializedException(Type type)
            : base(type, LogId.ContextAlreadyInitialized, "Context of type {0} already exists!", type)
        {
        }

        private ContextAlreadyInitializedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
