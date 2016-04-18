using System;
using System.Runtime.Serialization;
using CommonBase;
using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class StorageMappingNotFoundException : CommonException
    {
        public StorageMappingNotFoundException(Type type, ObjectId productItemId)
            : base(type, LogId.MappingNotFound, "Storage mapping for item '{0}' not found.", productItemId)
        {
        }

        private StorageMappingNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}