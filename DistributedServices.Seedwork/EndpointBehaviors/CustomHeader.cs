using Infrastructure.Crosscutting.Framework.Utils;
using System.IO;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

namespace DistributedServices.Seedwork.EndpointBehaviors
{
    public class CustomHeader : MessageHeader
    {
        private ServiceHeader _customData;

        public ServiceHeader CustomData { get { return _customData; } }

        public CustomHeader()
        { }

        public CustomHeader(ServiceHeader customData)
        {
            _customData = customData;
        }

        protected override void OnWriteHeaderContents(System.Xml.XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var serializer = new XmlSerializer(typeof(ServiceHeader));

            var textWriter = new StringWriter();

            serializer.Serialize(textWriter, _customData);

            textWriter.Close();

            string text = textWriter.ToString();

            writer.WriteElementString(CustomHeaderNames.CustomHeaderName, CustomHeaderNames.KeyName, text.Trim());
        }

        public override string Name
        {
            get { return (CustomHeaderNames.CustomHeaderName); }
        }

        public override string Namespace
        {
            get { return (CustomHeaderNames.CustomHeaderNamespace); }
        }
    }
}
