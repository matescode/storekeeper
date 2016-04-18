using System;
using System.Runtime.Serialization;

using CommonBase;
using CommonBase.Exceptions;

namespace StoreKeeper.Client.Exceptions
{
    [Serializable]
    public class ProductArticleNotFoundException : CommonException
    {
        public ProductArticleNotFoundException(Type type, string code)
            : base(type, LogId.ArticleNotFound, "Product article with code '{0}' does not exist.", code)
        {
        }

        public ProductArticleNotFoundException(Type type, ObjectId productId)
            : base(type, LogId.ArticleNotFound, "Product article with ID '{0}' does not exist.", productId)
        {
        }

        private ProductArticleNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}