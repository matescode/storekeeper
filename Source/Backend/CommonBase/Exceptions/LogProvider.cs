namespace CommonBase.Exceptions
{
    public sealed class LogProvider
    {
        private static LogProvider _instance = null;

        private LogProvider(IExceptionLogger logger)
        {
            Logger = logger;
        }

        #region Properties

        public static LogProvider Instance
        {
            get
            {
                return _instance;
            }
        }

        public IExceptionLogger Logger
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public static void Initialize(IExceptionLogger logger)
        {
            if (_instance == null)
            {
                _instance = new LogProvider(logger);
            }
        }

        public static void Close()
        {
            if (_instance != null)
            {
                _instance.Logger = null;
                _instance = null;
            }
        }

        #endregion Methods
    }
}