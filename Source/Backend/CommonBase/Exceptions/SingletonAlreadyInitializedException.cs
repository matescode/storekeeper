using System;
using System.Runtime.Serialization;

namespace CommonBase.Exceptions
{
    [Serializable]
    public class SingletonAlreadyInitializedException : CommonException
    {
        public SingletonAlreadyInitializedException(Type type)
            : base(type, LogId.SingletonAlreadyInitialized, "Singleton instance of type {0} already exists!", type)
        {
        }

        protected SingletonAlreadyInitializedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}