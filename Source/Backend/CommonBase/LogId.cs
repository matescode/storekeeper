namespace CommonBase
{
    public class LogId
    {
        private const int CommonBaseId = 1;

        public const int SingletonAlreadyInitialized = CommonBaseId + 1;

        public const int ObjectDisposed = CommonBaseId + 2;

        protected const int LastBaseId = 200;

        protected const int LastSystemId = 1000;
    }
}