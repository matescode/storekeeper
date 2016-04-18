namespace StoreKeeper.Server
{
    public static class DataManagerFactory
    {
        public static IDataManager CreateDataManager()
        {
            return new DataManager();
        }
    }
}