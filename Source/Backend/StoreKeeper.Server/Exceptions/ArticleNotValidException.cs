using System;

namespace StoreKeeper.Server.Exceptions
{
    public class ArticleNotValidException : InternalServerException
    {
        public ArticleNotValidException(Type type)
            : base(type, LogId.ArticleParsingFault, "Article cannot be parsed.")
        {
        }
    }
}