using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class RepositoryNotInitializedException : CommonException
    {
        public RepositoryNotInitializedException(Type type)
            : base(type, LogId.ClientRepositoryNotInitialized, "Client repository is not initialized.")
        {
        }

        private RepositoryNotInitializedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}