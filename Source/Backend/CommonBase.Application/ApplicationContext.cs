using CommonBase.Exceptions;
using CommonBase.Log;
using System;

namespace CommonBase.Application
{
    /// <summary>
    /// Application context provides services accessible in any place of application and at any time.
    /// </summary>
    public abstract class ApplicationContext : ContextInstance<ApplicationContext>, IExceptionLogger, IServiceProvider
    {
        private readonly ApplicationType _applicationType;
        private readonly ILog _log;
        private ApplicationConfiguration _config;
        private readonly IApplicationInfo _applicationInfo;

        protected ApplicationContext(ApplicationType applicationType, ILog log)
        {
            _applicationType = applicationType;
            _log = log;

            _applicationInfo = CreateApplicationInfo();

            _config = CreateConfiguration();

            // inject logging to common library
            LogProvider.Initialize(this);
        }

        #region Properties

        public static ILog Log
        {
            get
            {
                return Instance._log;
            }
        }

        public static ApplicationConfiguration Config
        {
            get
            {
                return Instance._config;
            }
        }


        public static ApplicationType ApplicationType
        {
            get
            {
                return Instance._applicationType;
            }
        }

        public static IApplicationInfo Info
        {
            get
            {
                return Instance._applicationInfo;
            }
        }

        public static string RootPath
        {
            get
            {
                return Instance.GetApplicationPath();
            }
        }

        public static string RegistryRoot
        {
            get
            {
                return Instance.GetRegistryRoot();
            }
        }

        #endregion Properties

        #region Public Methods

        public static void Close()
        {
            Instance.OnClosing();
            Instance._config.Close();
            Instance._config = null;
            LogProvider.Close();
            Log.Close();
            CloseInstance();
        }

        #endregion

        #region Internals and Helpers

        protected abstract ApplicationConfiguration CreateConfiguration();

        protected virtual IApplicationInfo CreateApplicationInfo()
        {
            return new ApplicationInfo();
        }

        // TODO:
        protected virtual string GetApplicationPath()
        {
            return string.Empty;
        }

        // TODO:
        protected virtual string GetRegistryRoot()
        {
            return string.Empty;
        }

        protected virtual void OnClosing()
        {
        }

        #endregion

        #region Overrides

        public override T GetService<T>()
        {
            if (typeof(T) == typeof(IExceptionLogger))
            {
                return Instance as T;
            }
            return base.GetService<T>();
        }

        #endregion

        #region IExceptionLogger Methods

        public void LogException(Type type, int id, string message, Exception exception)
        {
            if (exception != null)
            {
                Log.Error(type, id, exception);
            }
            else
            {
                Log.Error(type, id, message);
            }
        }

        #endregion IExceptionLogger Methods
    }
}