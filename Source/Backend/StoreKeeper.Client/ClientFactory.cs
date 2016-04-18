using StoreKeeper.Common;

namespace StoreKeeper.Client
{
    public static class ClientFactory
    {
        public static IStoreKeeperServiceClient CreateClient(IClientConfiguration clientConfiguration)
        {
            return new StoreKeeperServiceClient(clientConfiguration);
        }
    }
}