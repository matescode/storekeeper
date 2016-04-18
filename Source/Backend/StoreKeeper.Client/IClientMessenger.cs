namespace StoreKeeper.Client
{
    public interface IClientMessenger
    {
        void ConnectionClosing();

        void ConnectionRestarted();

        void DatabaseStatusChanged();

        bool CalculationRequested { set; }

        bool DataUpdated { set; }
    }
}