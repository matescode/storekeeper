using System;
using System.Runtime.Serialization;

namespace CommonBase.Exceptions
{
    [Serializable]
    public class ObjectAlreadyDisposedException : CommonException
    {
         public ObjectAlreadyDisposedException(Type type)
            : base(type, LogId.ObjectDisposed, "Object of type {0} is already disposed!", type)
        {
        }

         protected ObjectAlreadyDisposedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}