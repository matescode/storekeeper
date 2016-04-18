using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class CannotPerformMaterialTransferException : CommonException
    {
        public CannotPerformMaterialTransferException(Type type, string code, string storage) 
            : base(type, LogId.CannotPerformTransfer, "Cannot perform transfer of material '{0}' to storage '{1}'.", code, storage)
        {
        }

        private CannotPerformMaterialTransferException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}