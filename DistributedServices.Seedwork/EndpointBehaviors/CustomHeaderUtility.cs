using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.IO;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;

namespace DistributedServices.Seedwork.EndpointBehaviors
{
    public static class CustomHeaderUtility
    {
        public static ServiceHeader ReadHeader(OperationContext operationContext)
        {
            var customData = new ServiceHeader
            {
                ApplicationUserName = "Swiftfin_Dev",
                ApplicationDomainName = "TestDomain",
                EnvironmentUserName = "JohnDoe",
                EnvironmentProcessorId = "john",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMACAddress = "000000",
                EnvironmentMotherboardSerialNumber = "000000",
                EnvironmentDomainName = Environment.UserDomainName,
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentIPAddress = "127.0.0.1"
            };
            if (operationContext == null)
                return customData;

            var messageHeaders = operationContext.IncomingMessageHeaders;

            var headerPosition = messageHeaders.FindHeader(CustomHeaderNames.CustomHeaderName, CustomHeaderNames.CustomHeaderNamespace);

            if (headerPosition == -1)
                return customData;

            var content = messageHeaders.GetHeader<XmlNode[]>(headerPosition);

            var text = content[0].InnerText;

            var serializer = new XmlSerializer(typeof(ServiceHeader));

            using (var textReader = new StringReader(text))
            {
                customData = (ServiceHeader)serializer.Deserialize(textReader);
            }

            return customData;
        }
    }
}
