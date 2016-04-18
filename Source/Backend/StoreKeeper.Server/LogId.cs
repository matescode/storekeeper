namespace StoreKeeper.Server
{
    public class LogId : CommonBase.LogId
    {
        private const int StoreKeeperServerId = LastSystemId + 200;

        public const int SupplierParsingFault = StoreKeeperServerId + 1;

        public const int AccountingDataThresholdReached = StoreKeeperServerId + 2;

        public const int AccountingDataExportFinished = StoreKeeperServerId + 3;

        public const int ArticleParsingFault = StoreKeeperServerId + 4;

        public const int DataUpdatedNotification = StoreKeeperServerId + 5;

        public const int DatabaseLockChanged = StoreKeeperServerId + 6;
    }
}