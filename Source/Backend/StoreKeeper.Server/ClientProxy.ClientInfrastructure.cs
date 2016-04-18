namespace StoreKeeper.Server
{
    public partial class ClientProxy
    {
        #region IClientInfrastructure Implementation

        public string ValidateConnection(string ticket)
        {
            return Channel.ValidateConnection(ticket);
        }

        public bool ClosingConnection()
        {
            return Channel.ClosingConnection();
        }

        public void ConnectionRestarted()
        {
            Channel.ConnectionRestarted();
        }

        public bool IsActive()
        {
            return Channel.IsActive();
        }

        public void DataUpdated()
        {
            Channel.DataUpdated();
        }

        public void DatabaseLockChanged()
        {
            Channel.DatabaseLockChanged();
        }

        #endregion
    }
}