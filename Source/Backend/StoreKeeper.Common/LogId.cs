namespace StoreKeeper.Common
{
    public class LogId : CommonBase.LogId
    {
        private const int StoreKeeperCommonId = LastSystemId + 300;

        public const int ClientNotValid = StoreKeeperCommonId + 1;
        public const int CannotCreateSession = StoreKeeperCommonId + 2;
        public const int ServiceContractFault = StoreKeeperCommonId + 3;
        public const int UserNotRegistered = StoreKeeperCommonId + 4;
        public const int SessionNotRegistered = StoreKeeperCommonId + 5;
        public const int NotAuthorizedRequest = StoreKeeperCommonId + 6;
        public const int AccountDataSyncFault = StoreKeeperCommonId + 7;
    }
}