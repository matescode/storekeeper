using System;
using System.Runtime.Serialization;

using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class MaterialArticleNotFoundException : CommonException
    {
        public MaterialArticleNotFoundException(Type type, string code)
            : base(type, LogId.ArticleNotFound, "Material article with code '{0}' does not exist.", code)
        {
        }

        private MaterialArticleNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}