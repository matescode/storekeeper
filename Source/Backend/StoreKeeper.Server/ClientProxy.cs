using System;
using System.ServiceModel.Channels;
using StoreKeeper.Common;

namespace StoreKeeper.Server
{
    public partial class ClientProxy : ServiceProxy<IClientInfrastructure>, IClientInfrastructure
    {
        public ClientProxy(IServiceDescriptor serviceDescriptor)
            : base(serviceDescriptor, Constants.ClientAccessContract)
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