namespace StoreKeeper.Common
{
    public class ConnectionStringHolder
    {
        private static ConnectionStringHolder _instance;
        private string _connectionString;

        private ConnectionStringHolder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void Initialize(string connectionString)
        {
            if (_instance == null)
            {
                _instance = new ConnectionStringHolder(connectionString);
            }
        }

        public static void Close()
        {
            if (_instance != null)
            {
                _instance._connectionString = null;
                _instance = null;
            }
        }

        public static string Value
        {
            get { return _instance._connectionString; }
        }
    }
}