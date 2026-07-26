using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ServiceModel.Description;

namespace DistributedServices.Seedwork.EndpointBehaviors
{
    public class CustomBehavior : IEndpointBehavior
    {
        private readonly ServiceHeader _serviceHeader;

        public CustomBehavior(ServiceHeader serviceHeader)
        {
            if (serviceHeader == null)
                throw new ArgumentNullException("serviceHeader");

            _serviceHeader = serviceHeader;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            CustomMessageInspector inspector = new CustomMessageInspector(_serviceHeader);

            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
}
