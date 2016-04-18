using System;
using System.ServiceModel.Channels;
using StoreKeeper.Common;

namespace StoreKeeper.Client
{
    public partial class StoreKeeperServiceProxy : ServiceProxy<IServerAccess>, IServerAccess
    {
        public StoreKeeperServiceProxy(IServiceDescriptor serviceDescriptor)
            : base(serviceDescriptor, Constants.ServiceAccessContract)
        {
        }

        #region Overrides

        protected override Binding CreateBinding(bool secured)
        {
            Binding binding = Infrastructure.CreateApplicationBinding(false);
            binding.SendTimeout = TimeSpan.FromMinutes(Constants.DefaultProxyTimeoutInMinutes);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(Constants.DefaultProxyTimeoutInMinutes);
            return binding;
        }

        #endregion
    }
}