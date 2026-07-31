using System.Configuration;

namespace SwiftFinancials.AccountAlertDispatcher.Configuration
{
    public class AccountAlertDispatcherSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("uniqueId", IsKey = true, IsRequired = true)]
        public string UniqueId
        {
            get { return ((string)(base["uniqueId"])); }
            set { base["uniqueId"] = value; }
        }

        [ConfigurationProperty("templatesPath", IsKey = true, IsRequired = true)]
        public string TemplatesPath
        {
            get { return ((string)(base["templatesPath"])); }
            set { base["templatesPath"] = value; }
        }

        [ConfigurationProperty("enabled", IsKey = false, IsRequired = true)]
        public int Enabled
        {
            get { return ((int)(base["enabled"])); }
            set { base["enabled"] = value; }
        }
    }
}
