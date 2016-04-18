using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class ContextNotInitializedException : CommonException
    {
        public ContextNotInitializedException(Type type, string contextName)
            : base(type, LogId.ClientDataContextNotInitialized, "Client data context '{0}' is not initialized.", contextName)
        {
        }

        private ContextNotInitializedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}