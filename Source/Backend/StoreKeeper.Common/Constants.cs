using System;

namespace StoreKeeper.Common
{
    public class Constants
    {
        public const string ClientConnectionHash = "#client#";

        public const string ClientValidResultHash = "#ok#";

        public const string ServiceHostServiceName = "StoreKeeperService";

        public const string ClientHostServiceName = "StoreKeeperClient";

        public const string ServiceAccessContract = "ServiceAccess";

        public const string ClientAccessContract = "ClientAccess";

        public const string LastUpdateSystemValue = "LastUpdate";

        public const string ResponsibleUser = "ResponsibleUser";

        public const string LockedBy = "LockedBy";

        public const double DefaultProxyTimeoutInMinutes = 120;

        public static readonly Guid CentralStorageId = new Guid("D67BFF48-E36F-4066-888B-23BA5E622FB0");
    }
}