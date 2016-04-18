namespace StoreKeeper.Client
{
    public class UserContext
    {
        private static UserContext _instance;

        private UserContext(string userId)
        {
            UserId = userId;
        }

        public static string UserId { get; private set; }

        public static void Initialize(string userId)
        {
            if (_instance == null)
            {
                _instance = new UserContext(userId);
            }
        }

        public static void Close()
        {
            UserId = null;
            _instance = null;
        }
    }
}