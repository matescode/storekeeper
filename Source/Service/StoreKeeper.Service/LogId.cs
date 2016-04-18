namespace StoreKeeper.Service
{
    public sealed class LogId : Common.LogId
    {
        private const int ServiceId = LastSystemId + 100;

        public const int Request = ServiceId + 1;
    }
}