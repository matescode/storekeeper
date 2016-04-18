using System;
using System.ServiceModel;
using CommonBase.Utils;

namespace StoreKeeper.Client
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    internal partial class ClientContract
    {
        private IClientInfrastructureCallback _callback;
        private readonly Func<bool> _checkAvailability;

        public ClientContract(IClientInfrastructureCallback callback, Func<bool> checkAvailability)
        {
            ArgumentValidator.IsNotNull("checkAvailability", checkAvailability, "Availability check function cannot be null.");

            _callback = callback;
            _checkAvailability = checkAvailability;
        }

        public void Close()
        {
            _callback = null;
        }
    }
}