using System;
using System.IO;
using System.ServiceProcess;
using CommonBase.Application;
using CommonBase.Log;

namespace CommonBase.Service
{
    public abstract class WorkerService : ServiceBase
    {
        public const string StopCommand = "stop";

        protected static void RunService(WorkerService service, string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            ServiceSettings settings = new ServiceSettings(args);
            RunService(service, settings);
        }

        #region Abstract Methods

        protected abstract ILog CreateLog();

        protected abstract ApplicationContext CreateApplicationContext(ILog log);

        protected abstract ServiceContext CreateServiceContext();

        protected abstract void Create();

        protected abstract void Destroy();

        #endregion

        #region Internals and Helpers

        private static void RunService(WorkerService service, ServiceSettings settings)
        {
            service.Initialize();
            if (settings.Mode == ServiceMode.Service)
            {
                Run(service);
            }
            else if (settings.Mode == ServiceMode.Console)
            {
                service.OnStart(null);

                Console.WriteLine("Type '{0}' to stop ...", StopCommand);

                while (Console.ReadLine() != StopCommand)
                {
                }

                service.OnStop();
            }
        }

        private void Initialize()
        {
            ILog log = CreateLog();

            ApplicationContext appContext = CreateApplicationContext(log);
            if (appContext == null)
            {
                throw new InvalidOperationException("Application context cannot be null.");
            }

            ServiceContext serviceContext = CreateServiceContext();
            if (serviceContext == null)
            {
                throw new InvalidOperationException("Service context cannot be null.");
            }
        }

        #endregion

        #region Overrides

        protected override void OnStart(string[] args)
        {
            Create();
        }

        protected override void OnStop()
        {
            Destroy();
            ServiceContext.Close();
            ApplicationContext.Close();
        }

        #endregion
    }
}