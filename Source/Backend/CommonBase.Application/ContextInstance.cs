using CommonBase.Utils;

namespace CommonBase.Application
{
    public abstract class ContextInstance<TInstance> : SingletonInstance<TInstance>
        where TInstance : ContextInstance<TInstance>
    {
        protected ContextInstance()
        {
        }

        #region Methods

        public static TService Service<TService>() where TService : class
        {
            TService service = Instance.GetService<TService>();
            return service ?? DefaultServiceProvider.GetDefaultProvider<TService>(Instance.GetType());
        }

        public virtual T GetService<T>() where T : class
        {
            return null;
        }

        #endregion Methods
    }
}