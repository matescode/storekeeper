using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class MaterialOrderAlreadyExistException : CommonException
    {
        public MaterialOrderAlreadyExistException(Type type, string code)
            : base(type, LogId.ArticleOrderAlreadyExists, "Order of material with code '{0}' already exists.", code)
        {
        }

        private MaterialOrderAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}