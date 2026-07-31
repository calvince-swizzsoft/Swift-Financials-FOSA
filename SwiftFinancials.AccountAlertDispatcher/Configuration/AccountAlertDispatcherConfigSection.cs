using System.Configuration;

namespace SwiftFinancials.AccountAlertDispatcher.Configuration
{
    public class AccountAlertDispatcherConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("accountAlertDispatcherSettings")]
        public AccountAlertDispatcherSettingsCollection AccountAlertDispatcherSettingsItems
        {
            get { return ((AccountAlertDispatcherSettingsCollection)(base["accountAlertDispatcherSettings"])); }
        }
    }
}
