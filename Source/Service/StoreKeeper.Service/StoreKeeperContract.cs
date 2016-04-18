using System.ServiceModel;

using CommonBase.Application;
using CommonBase.Log;
using StoreKeeper.Common;

namespace StoreKeeper.Service
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class StoreKeeperContract
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof (StoreKeeperContract));

        #region IApplicationContract Implementation

        public string GetContractVersion()
        {
            return ApplicationContext.Info.Version.ToString();
        }

        #endregion

        #region Internals and Helpers

        private void LogRequest(string requestName, string clientComputer)
        {
            Logger.Info(LogId.Request, "Incomming request '{0}' from computer '{1}'.", requestName, clientComputer);
        }

        private void LogRequest(string requestName, SessionId sessionId)
        {
            Logger.Info(LogId.Request, "Incomming request '{0}' from computer identified by session '{1}'.", requestName, sessionId);
        }

        #endregion
    }
}