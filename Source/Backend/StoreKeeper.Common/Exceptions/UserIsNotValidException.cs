using System;
using System.Runtime.Serialization;
using CommonBase.Exceptions;

namespace StoreKeeper.Common.Exceptions
{
    [Serializable]
    public class UserIsNotValidException : CommonException
    {
         public UserIsNotValidException(Type type, string username)
            : base(type, LogId.UserNotRegistered, "User {0} is not registerred or password is incorrect.", username)
        {
        }

         protected UserIsNotValidException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}