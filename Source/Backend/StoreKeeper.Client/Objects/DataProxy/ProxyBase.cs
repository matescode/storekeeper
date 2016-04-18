namespace StoreKeeper.Client.Objects.DataProxy
{
    internal abstract class ProxyBase
    {
        protected ProxyBase(IDataChange dataChange)
        {
            DataChange = dataChange;
        }

        protected IDataChange DataChange { get; private set; }

        internal abstract void Load();
    }
}