using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Common.Exceptions
{
    [Serializable]
    public class ServiceContractException : CommonException
    {
        public ServiceContractException(Type type, string message)
            : base(type, LogId.ServiceContractFault, message)
        {
        }

        protected ServiceContractException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}