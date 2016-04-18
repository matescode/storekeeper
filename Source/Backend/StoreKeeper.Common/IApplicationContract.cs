using System.ServiceModel;

namespace StoreKeeper.Common
{
    [ServiceContract]
    public interface IApplicationContract
    {
        [OperationContract]
        string GetContractVersion();
    }
}