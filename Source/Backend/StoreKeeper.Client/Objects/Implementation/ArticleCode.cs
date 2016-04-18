using System;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ArticleCode : IArticleCode
    {
        public ArticleCode(string code, string description)
        {
            Code = code;
            Description = description;
        }

        #region Implementation

        public string Code { get; private set; }

        public string Description { get; private set; }

        #endregion
    }
}