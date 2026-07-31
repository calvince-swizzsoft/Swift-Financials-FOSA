using System.Configuration;

namespace SwiftFinancials.EmailAlertDispatcher.Configuration
{
    public class EmailDispatcherConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("emailDispatcherSettings")]
        public EmailDispatcherSettingsCollection EmailDispatcherSettingsItems
        {
            get { return ((EmailDispatcherSettingsCollection)(base["emailDispatcherSettings"])); }
        }
    }
}
