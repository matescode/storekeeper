using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.Common
{
    public class CommonDataHelper
    {
        private const string CardType = "Card";

        private const string ProductType = "Product";

        public static ArticleType ConvertArticleType(string articleTypeStr)
        {
            if (articleTypeStr.ToUpper() == CardType.ToUpper())
            {
                return ArticleType.Card;
            }
            
            if (articleTypeStr.ToUpper() == ProductType.ToUpper())
            {
                return ArticleType.Product;
            }

            return ArticleType.Unknown;
        }
    }
}