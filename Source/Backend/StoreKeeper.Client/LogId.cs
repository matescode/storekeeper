namespace StoreKeeper.Client
{
    public class LogId : CommonBase.LogId
    {
        private const int StoreKeeperClientId = LastSystemId + 400;

        public const int ClientDataContextNotInitialized = StoreKeeperClientId + 1;
        
        public const int ClientRepositoryNotInitialized = StoreKeeperClientId + 2;

        public const int ArticleNotFound = StoreKeeperClientId + 3;

        public const int MappingNotFound = StoreKeeperClientId + 4;

        public const int CannotPerformTransfer = StoreKeeperClientId + 5;

        public const int ArticleOrderAlreadyExists = StoreKeeperClientId + 6;

        public const int DatabaseLocked = StoreKeeperClientId + 7;
    }
}