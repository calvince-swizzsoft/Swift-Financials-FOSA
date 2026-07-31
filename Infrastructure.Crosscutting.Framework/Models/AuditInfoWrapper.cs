using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Infrastructure.Crosscutting.Framework.Models
{
    [Serializable]
    public class AuditInfoWrapper
    {
        [XmlAttribute]
        public string TableName { get; set; }

        [XmlAttribute]
        public string EventType { get; set; }

        [XmlAttribute]
        public string RecordID { get; set; }

        public List<AuditInfo> AuditInfoCollection { get; set; }
    }
}
