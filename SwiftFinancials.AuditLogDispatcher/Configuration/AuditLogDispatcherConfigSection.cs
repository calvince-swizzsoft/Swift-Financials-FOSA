using System.Configuration;

namespace SwiftFinancials.AuditLogDispatcher.Configuration
{
    public class AuditLogDispatcherConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("auditLogDispatcherSettings")]
        public AuditLogDispatcherSettingsCollection AuditLogDispatcherSettingsItems
        {
            get { return (AuditLogDispatcherSettingsCollection)base["auditLogDispatcherSettings"]; }
        }
    }
}
