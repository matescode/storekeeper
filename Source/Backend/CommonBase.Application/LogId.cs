namespace CommonBase.Application
{
    public class LogId : CommonBase.LogId
    {
        private const int ApplicationId = LastBaseId + 100;

        public const int ContextAlreadyInitialized = ApplicationId + 1;
    }
}