using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

using CommonBase.Log;

using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts;
using StoreKeeper.Common.DataContracts.Sync;
using StoreKeeper.Server.Exceptions;

namespace StoreKeeper.Server
{
    internal class DataProcessor
    {
        private static ILogger Logger = LogManager.GetLogger(typeof(DataProcessor));

        private readonly List<ArticleData> _parsedArticles;
        private readonly List<ArticleItemData> _parsedItems;
        private AccountingDataSync _dataSync;

        public DataProcessor()
        {
            _parsedArticles = new List<ArticleData>();
            _parsedItems = new List<ArticleItemData>();
        }

        #region Properties

        public AccountingDataSync DataSync
        {
            get { return _dataSync; }
        }

        #endregion

        #region Public Methods

        public void ProcessFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Source XML file not found!", fileName);
            }

            _parsedArticles.Clear();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNode articleList = document.SelectSingleNode("//StockList");

            ParseArticles(articleList);
            _dataSync = new AccountingDataSync { Articles = _parsedArticles, ArticleItems = _parsedItems };
        }

        #endregion

        #region Internals and Helpers

        private void ParseArticles(XmlNode articleListNode)
        {
            XmlNodeList nodeList = articleListNode.SelectNodes("//Stockpile");
            if (nodeList == null)
            {
                return;
            }

            List<int> existing = new List<int>(5000);

            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlNode node = nodeList[i];

                ArticleData articleData = new ArticleData();

                int externalId;

                if (!int.TryParse(node.Attributes["Id"].Value, out externalId))
                {
                    throw new ArticleNotValidException(GetType());
                }

                if (existing.Contains(externalId))
                {
                    continue;
                }

                existing.Add(externalId);

                string name = GetArticleElementValue(node, "Name");
                string typeStr = GetArticleElementValue(node, "Type");
                string code = GetArticleElementValue(node, "Code");
                string specialCode = GetArticleElementValue(node, "SpecialCode");
                string countStr = GetArticleElementValue(node, "Count");
                string internalStorage = GetArticleElementValue(node, "InternalStorage");
                string purchPriceStr = GetArticleElementValue(node, "PurchasingPrice");
                string sellPriceStr = GetArticleElementValue(node, "SellingPrice");

                ArticleType articleType = CommonDataHelper.ConvertArticleType(typeStr);

                double count = GetDoubleValue(countStr);

                double purchPrice = GetDoubleValue(purchPriceStr);

                double sellPrice = GetDoubleValue(sellPriceStr);

                articleData.ExternalId = externalId;
                articleData.Name = name;
                articleData.Type = (int)articleType;
                articleData.Code = code;
                articleData.SpecialCode = specialCode;
                articleData.OriginalCount = count;
                articleData.InternalStorage = internalStorage;
                articleData.PurchasingPrice = purchPrice;
                articleData.SellingPrice = sellPrice;
                articleData.OrderCount = 0;

                if (articleType == ArticleType.Card)
                {
                    articleData.OrderName = GetArticleElementValue(node, "OrderName");

                    string orderedCountStr = GetArticleElementValue(node, "OrderedCount");
                    articleData.OrderCount = GetDoubleValue(orderedCountStr);
                }

                _parsedArticles.Add(articleData);

                if (articleType == ArticleType.Product)
                {
                    XmlNodeList itemList = node.SelectNodes("Items/Item");
                    if (itemList != null)
                    {
                        for (int j = 0; j < itemList.Count; ++j)
                        {
                            XmlNode itemNode = itemList[j];
                            int itemId = 0;
                            double quantity = 0.0;
                            string itemStorage = string.Empty;
                            string itemCode = string.Empty;
                            if (itemNode.Attributes != null)
                            {
                                if (!int.TryParse(itemNode.Attributes["Id"].Value, out itemId))
                                {
                                    throw new ArticleNotValidException(GetType());
                                }

                                itemCode = itemNode.Attributes["Code"].Value;
                                itemStorage = itemNode.Attributes["InternalStorage"].Value;

                                quantity = GetDoubleValue(itemNode.Attributes["Quantity"].Value);
                            }

                            _parsedItems.Add(new ArticleItemData
                                {
                                    ArticleDataId = articleData.Id,
                                    ExternalId = itemId,
                                    Code = itemCode,
                                    InternalStorage = itemStorage,
                                    Quantity = quantity
                                });
                        }
                    }
                }
            }
        }

        private string GetArticleElementValue(XmlNode node, string name)
        {
            XmlElement elem = node[name];
            if (elem == null)
            {
                throw new ArticleNotValidException(GetType());
            }
            return elem.InnerText;
        }

        private double GetDoubleValue(string valueStr, double defaultValue = 0)
        {
            NumberStyles style = NumberStyles.Number;
            double value;
            if (!double.TryParse(valueStr, style, CultureInfo.InvariantCulture, out value))
            {
                value = defaultValue;
            }
            return value;
        }

        #endregion
    }
}