using System;
using System.Xml.Serialization;

namespace Infrastructure.Crosscutting.Framework.Models
{
    [Serializable]
    public class AuditInfo
    {
        [XmlAttribute]
        public string ColumnName { get; set; }

        [XmlAttribute]
        public string OriginalValue { get; set; }

        [XmlAttribute]
        public string NewValue { get; set; }

        public override string ToString()
        {
            return string.Format("OriginalValue:{0}{1}NewValue:{2}", OriginalValue, Environment.NewLine, NewValue);
        }
    }
}
