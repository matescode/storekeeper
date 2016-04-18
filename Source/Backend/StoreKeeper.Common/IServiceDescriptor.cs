namespace StoreKeeper.Common
{
    public interface IServiceDescriptor
    {
        bool Secured { get; }

        string Server { get; }

        int Port { get; }

        string ServiceName { get; }
    }
}