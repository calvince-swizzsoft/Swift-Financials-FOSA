using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace DistributedServices.Seedwork.EndpointBehaviors
{
    public class CustomMessageInspector : IClientMessageInspector
    {
        private readonly ServiceHeader _serviceHeader;

        public CustomMessageInspector(
            ServiceHeader serviceHeader)
        {
            if (serviceHeader == null)
                throw new ArgumentNullException("serviceHeader");

            _serviceHeader = serviceHeader;
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {

        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            // Collect requisite client params
            var systemInfo = new SystemInfo();
            _serviceHeader.EnvironmentIPAddress = systemInfo.IPAddress;
            _serviceHeader.EnvironmentMACAddress = systemInfo.MACAddress;
            _serviceHeader.EnvironmentMotherboardSerialNumber = systemInfo.MotherboardSerialNumber;
            _serviceHeader.EnvironmentProcessorId = systemInfo.ProcessorId;
            _serviceHeader.EnvironmentUserName = systemInfo.UserName;
            _serviceHeader.EnvironmentMachineName = systemInfo.MachineName;
            _serviceHeader.EnvironmentDomainName = systemInfo.DomainName;
            _serviceHeader.EnvironmentOSVersion = Environment.OSVersion.ToString();

#if !SILVERLIGHT
            var principal = Thread.CurrentPrincipal;

            if (principal != null)
            {
                _serviceHeader.ApplicationUserName = principal.Identity.Name;
            }
#endif
            var header = new CustomHeader(_serviceHeader);

            // Add the custom header to the request.
            request.Headers.Add(header);

            return null;
        }
    }
}
