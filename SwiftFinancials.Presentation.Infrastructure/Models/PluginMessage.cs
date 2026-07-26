using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    [Serializable]
    public class PluginMessage
    {
        public Guid PluginId { get; set; }

        public string PluginName { get; set; }

        public string Content { get; set; }

        public string FormattedContent { get; set; }

        public int MessageType { get; set; }

        public string MessageTypeDescription { get { return Enum.IsDefined(typeof(PluginMessageType), MessageType) ? EnumHelper.GetDescription((PluginMessageType)MessageType) : string.Empty; } }

        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            return string.Format("{0}->{1}->{2}->{3}", PluginName, MessageTypeDescription, Content, CreatedDate);
        }
    }
}
