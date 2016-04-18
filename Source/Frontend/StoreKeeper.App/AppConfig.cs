using System;

using CommonBase.Application;

using StoreKeeper.Client;
using StoreKeeper.Common;

namespace StoreKeeper.App
{
    public class AppConfig : ApplicationConfiguration, IClientConfiguration
    {
        public AppConfig()
            : base(ConfigLoadType.Immediate)
        {
        }

        #region IClientConfiguration Implementation

        public string ServerName
        {
            get { return Get("Server"); }
            set { Set("Server", value); }
        }

        public int ServerPort
        {
            get { return Get("ServerPort", Convert.ToInt32, () => 8601); }
            set { Set("ServerPort", value); }
        }

        public string User
        {
            get { return Get("User"); }
            set { Set("User", value); }
        }

        public string SecurityToken
        {
            get { return Get("SecurityToken"); }
            set { Set("SecurityToken", value); }
        }

        public int ClientPort
        {
            get { return Get("ClientPort", Convert.ToInt32, () => 8610); }
            set { Set("ClientPort", value); }
        }

        public int SeekCodeCharLimit
        {
            get { return Get("SeekCode", Convert.ToInt32, () => 3); }
            set { Set("SeekCode", value); }
        }

        public bool IsOffline
        {
            get { return Get("Offline", Convert.ToBoolean, () => true); }
        }

        public bool NeedsCalculation
        {
            get { return Get("NeedCalc", Convert.ToBoolean, () => false); }
            set { Set("NeedCalc", value); }
        }

        public string DeliveryNoteCreatedByName
        {
            get { return Get("DNCreatedBy", () => "Radek Stebnický"); }
            set { Set("DNCreatedBy", value); }
        }

        public string DeliveryNoteCreatedByMail
        {
            get { return Get("DNCreatedByMail", () => "technik@flajzar.cz"); }
            set { Set("DNCreatedByMail", value); }
        }

        public int CurrentYear
        {
            get { return Get("CurrentYear", Convert.ToInt32, () => DateTime.Now.Year); }
            set { Set("CurrentYear", value); }
        }

        public string ConnectionString
        {
            get { return Get("ConnectionString", () => String.Empty); }
            set { Set("ConnectionString", value); }
        }

        #endregion

        #region IServiceDescriptor Implementation

        public bool Secured
        {
            get { return false; }
        }

        public string Server
        {
            get { return ServerName; }
        }

        public int Port
        {
            get { return ServerPort; }
        }

        public string ServiceName
        {
            get { return Constants.ServiceHostServiceName; }
        }

        #endregion
    }
}