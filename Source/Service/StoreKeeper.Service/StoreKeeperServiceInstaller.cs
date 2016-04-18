using CommonBase.Application;
using CommonBase.Service;

namespace StoreKeeper.Service
{
    public class StoreKeeperServiceInstaller : ServiceInstaller
    {
        public StoreKeeperServiceInstaller()
            : base(new ServiceApplicationInfo())
        {
        }
    }
}