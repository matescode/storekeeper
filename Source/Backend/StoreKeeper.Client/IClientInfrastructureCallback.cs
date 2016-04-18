namespace StoreKeeper.Client
{
    internal interface IClientInfrastructureCallback
    {
        void OnConnectionClosing();

        void OnConnectionRestarted();

        void DataUpdated();

        void DatabaseLockChanged();
    }
}