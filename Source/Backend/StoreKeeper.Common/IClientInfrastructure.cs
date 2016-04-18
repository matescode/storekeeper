using System.ServiceModel;

namespace StoreKeeper.Common
{
    [ServiceContract]
    public interface IClientInfrastructure : IApplicationContract
    {
        [OperationContract]
        string ValidateConnection(string ticket);

        [OperationContract]
        bool ClosingConnection();

        [OperationContract]
        void ConnectionRestarted();

        [OperationContract]
        bool IsActive();

        [OperationContract]
        void DataUpdated();

        [OperationContract]
        void DatabaseLockChanged();
    }
}