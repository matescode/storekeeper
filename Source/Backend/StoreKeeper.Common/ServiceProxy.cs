using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using CommonBase.Exceptions;
using CommonBase.Utils;

namespace StoreKeeper.Common
{
    public abstract class ServiceProxy<TContract> : EasyDispose, IApplicationContract
        where TContract : class, IApplicationContract
    {
        private ChannelFactory<TContract> _factory;
        private readonly object _sync = new object();
        private Binding _binding;
        private EndpointAddress _endpoint;
        private TContract _channel;

        protected ServiceProxy(IServiceDescriptor serviceDescriptor, string contract)
        {
            ServiceDescriptor = serviceDescriptor;
            Contract = contract;
            Name = string.Format("{0}/{1}", serviceDescriptor.ServiceName, Contract);
        }

        #region Properties

        protected IServiceDescriptor ServiceDescriptor { get; private set; }

        protected string Contract { get; private set; }

        protected string Name { get; private set; }

        public TContract Channel
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectAlreadyDisposedException(GetType());
                }
                return ThreadHelper.EnsureInitialized(ref _channel, () => Factory.CreateChannel(), _sync);
            }
        }

        private ChannelFactory<TContract> Factory
        {
            get
            {
                return ThreadHelper.EnsureInitialized(
                    ref _factory,
                    () =>
                    {
                        _binding = CreateBinding(ServiceDescriptor.Secured);
                        _endpoint = new EndpointAddress(Infrastructure.GetEndpointUrl(ServiceDescriptor, Contract));
                        ChannelFactory<TContract> factory = new ChannelFactory<TContract>(_binding, _endpoint);
                        foreach (var operation in factory.Endpoint.Contract.Operations)
                        {
                            DataContractSerializerOperationBehavior dataContractBehavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                            if (dataContractBehavior != null)
                            {
                                dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                            }
                        }
                        return factory;
                    },
                    _sync);
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract Binding CreateBinding(bool secured);

        #endregion

        #region IApplicationContract Implementation

        public string GetContractVersion()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overrides

        protected override void DisposeAction()
        {

        }

        #endregion
    }
}