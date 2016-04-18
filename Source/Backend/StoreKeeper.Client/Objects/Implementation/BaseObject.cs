using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal abstract class BaseObject<TProxy> where TProxy : ProxyBase
    {
        protected BaseObject(TProxy proxy)
        {
            Proxy = proxy;
            Load();
        }

        protected TProxy Proxy { get; private set; }

        public void Load()
        {
            Proxy.Load();
        }
    }
}