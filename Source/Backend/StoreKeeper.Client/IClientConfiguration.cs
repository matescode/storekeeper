using StoreKeeper.Common;

namespace StoreKeeper.Client
{
    public interface IClientConfiguration : IServiceDescriptor
    {
        string ServerName { get; set; }

        int ServerPort { get; set; }

        string User { get; set; }

        string SecurityToken { get; set; }

        int ClientPort { get; set; }

        int SeekCodeCharLimit { get; set; }

        bool IsOffline { get; }

        string ConnectionString { get; set; }
    }
}