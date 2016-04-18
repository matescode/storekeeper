namespace StoreKeeper.Server
{
    internal class ServerSettings
    {
        public const string AccountingDataCommandPath = @"ExpotStorage.cmd";

        public const string AccountingDataTemplateFile = @"Storage.xml";

        public const string AccountingDataResultFile = @"Storage_data.xml_trn_o1.xml";

        public const string AccountingDataTemplateMatch = @"**TIMESTAMP**";

        public const string AccountingDataRequestTemplate = "RequestTemplate.xml";

        public const int DataParserCheckFileSleep = 2000;

        public const int DataParserSafetySleep = 2000;
    }
}