using System;
using System.ServiceModel;

using CommonBase.Log;
using CommonBase.Log.Logs;
using CommonBase.Service;
using CommonBase.Application;

using StoreKeeper.Common;
using StoreKeeper.Server;

namespace StoreKeeper.Service
{
    internal partial class StoreKeeperService : WorkerService
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(StoreKeeperService));

        private ServiceHost _applicationHost;

        protected override ILog CreateLog()
        {
            return new SystemEventLog("StoreKeeperService", "StoreKeeper_ServiceLog");
        }

        protected override ApplicationContext CreateApplicationContext(ILog log)
        {
            Logger.Info("StoreKeeperService: Create application context.");
            return new Contexts.ApplicationContext(log);
        }

        protected override ServiceContext CreateServiceContext()
        {
            Logger.Info("StoreKeeperServicee: Create service context.");
            return new Contexts.ServiceContext();
        }

        protected override void Create()
        {
            ConnectionStringHolder.Initialize(Contexts.ApplicationContext.Config.ConnectionString);
            StoreKeeperServer.Create();
            Contexts.ServiceContext.Initialize();

            IServiceDescriptor serviceDescriptor = new ServiceConfiguration();

            string uriApplication = Infrastructure.GetEndpointUrl(serviceDescriptor);

            _applicationHost = new ServiceHost(typeof(StoreKeeperContract), new Uri(uriApplication));
            Infrastructure.AddServiceBehaviors(_applicationHost.Description);

            _applicationHost.AddServiceEndpoint(
                typeof (IServerAccess), 
                Infrastructure.CreateApplicationBinding(false),
                Constants.ServiceAccessContract
            );

            _applicationHost.Open();

            Logger.Info("StoreKeeperService started on address: {0}", uriApplication);

            Contexts.ServiceContext.SessionManager.ReconnectSessions();
        }

        protected override void Destroy()
        {
            if (_applicationHost != null)
            {
                _applicationHost.Close();
                _applicationHost = null;
            }
            StoreKeeperServer.Close();
            ConnectionStringHolder.Close();
            Logger.Info("StoreKeeperService destroyed.");
        }
    }
}