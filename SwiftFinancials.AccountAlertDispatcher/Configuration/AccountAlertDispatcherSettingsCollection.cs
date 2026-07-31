using System.Configuration;

namespace SwiftFinancials.AccountAlertDispatcher.Configuration
{
    public class AccountAlertDispatcherSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AccountAlertDispatcherSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AccountAlertDispatcherSettingsElement)(element)).UniqueId;
        }

        public AccountAlertDispatcherSettingsElement this[int index]
        {
            get { return (AccountAlertDispatcherSettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("queuePath", IsRequired = true)]
        public string QueuePath
        {
            get
            {
                return (string)base["queuePath"];
            }
        }

        [ConfigurationProperty("queueReceivers", IsRequired = true)]
        public int QueueReceivers
        {
            get
            {
                return (int)base["queueReceivers"];
            }
        }
    }
}
