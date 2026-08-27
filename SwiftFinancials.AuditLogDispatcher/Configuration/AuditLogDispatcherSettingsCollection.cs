using System.Configuration;

namespace SwiftFinancials.AuditLogDispatcher.Configuration
{
    public class AuditLogDispatcherSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() { return new AuditLogDispatcherSettingsElement(); }
        protected override object GetElementKey(ConfigurationElement element) { return ((AuditLogDispatcherSettingsElement)element).UniqueId; }
        public AuditLogDispatcherSettingsElement this[int index] { get { return (AuditLogDispatcherSettingsElement)BaseGet(index); } }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name { get { return (string)base["name"]; } }

        [ConfigurationProperty("queuePath", IsRequired = true)]
        public string QueuePath { get { return (string)base["queuePath"]; } }

        [ConfigurationProperty("queueReceivers", IsRequired = true)]
        public int QueueReceivers { get { return (int)base["queueReceivers"]; } }
    }
}
