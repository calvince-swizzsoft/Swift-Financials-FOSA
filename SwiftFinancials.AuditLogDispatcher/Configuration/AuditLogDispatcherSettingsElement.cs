using System.Configuration;

namespace SwiftFinancials.AuditLogDispatcher.Configuration
{
    public class AuditLogDispatcherSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("uniqueId", IsKey = true, IsRequired = true)]
        public string UniqueId { get { return (string)base["uniqueId"]; } set { base["uniqueId"] = value; } }

        [ConfigurationProperty("logMode", IsRequired = false, DefaultValue = 0)]
        public int LogMode { get { return (int)base["logMode"]; } set { base["logMode"] = value; } }

        [ConfigurationProperty("enabled", IsRequired = true)]
        public int Enabled { get { return (int)base["enabled"]; } set { base["enabled"] = value; } }
    }
}
