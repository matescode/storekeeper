using System;
using CommonBase.Application;

namespace StoreKeeper.Server
{
    public class Config : ApplicationConfiguration
    {
        public Config()
            : base(ConfigLoadType.Immediate)
        {
        }

        public string AccountingDataRootFolder
        {
            get { return Get("AccountingDataRootFolder"); }
        }

        public int SafetyThresholdInMinutes
        {
            get { return Get("SafetyThresholdInMinutes", Convert.ToInt32, () => 2); }
        }

        public int ServerPort
        {
            get { return Get("ServerPort", Convert.ToInt32, () => 8601); }
            set { Set("ServerPort", value); }
        }

        public string ConnectionString
        {
            get { return Get("ConnectionString", () => String.Empty); }
            set { Set("ConnectionString", value); }
        }
    }
}