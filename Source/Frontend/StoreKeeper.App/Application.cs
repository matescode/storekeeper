using System;
using CommonBase.Application;
using CommonBase.Resources;
using CommonBase.UI;
using StoreKeeper.Resources;

namespace StoreKeeper.App
{
    public class Application : UIApplication
    {
        protected override ApplicationContext CreateApplicationContext()
        {
            return new AppContext();
        }

        protected override ResourceLibrary CreateResourceLibrary()
        {
            return new StoreKeeperResourceLibrary();
        }
    }
}