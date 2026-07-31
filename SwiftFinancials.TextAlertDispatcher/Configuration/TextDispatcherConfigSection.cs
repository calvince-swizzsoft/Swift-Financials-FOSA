using System.Configuration;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Configuration
{
    public class TextDispatcherConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("textDispatcherSettings")]
        public TextDispatcherSettingsCollection TextDispatcherSettingsItems
        {
            get { return ((TextDispatcherSettingsCollection)(base["textDispatcherSettings"])); }
        }
    }
}
