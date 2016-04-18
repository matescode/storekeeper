using System;

namespace StoreKeeper.Service
{
    internal partial class StoreKeeperService
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                args = new[] { "--mode=Service" };
            }
            RunService(new StoreKeeperService(), args);
        }
    }
}