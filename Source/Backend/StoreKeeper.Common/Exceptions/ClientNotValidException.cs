using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Common.Exceptions
{
    [Serializable]
    public class ClientNotValidException : CommonException
    {
        public ClientNotValidException(Type type)
            : base(type, LogId.ClientNotValid, "Client library instance is not valid!")
        {
        }

        protected ClientNotValidException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}