using CommonBase.Application;

namespace CommonBase.Service
{
    public abstract class ServiceContext : ContextInstance<ServiceContext>
    {
        private readonly IServiceData _serviceData;

        protected ServiceContext()
        {
            _serviceData = new ServiceData();
        }

        #region Properties

        public static IServiceData ServiceData
        {
            get
            {
                return Instance._serviceData;
            }
        }

        #endregion

        #region Public Methods

        public static void Close()
        {
            Instance.OnClosing();
            ServiceData.Clear();
            CloseInstance();
        }

        #endregion

        #region Overrides

        public override T GetService<T>()
        {
            if (typeof(T) == typeof(IServiceData))
            {
                return Instance._serviceData as T;
            }
            return base.GetService<T>();
        }

        #endregion

        #region Internals and Helpers

        protected virtual void OnClosing()
        {
        }

        #endregion
    }
}