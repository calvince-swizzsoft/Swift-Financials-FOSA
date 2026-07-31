using System.Configuration;

namespace Infrastructure.Crosscutting.Framework.Configuration
{
    public class ServiceBrokerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("serviceBrokerSettings")]
        public ServiceBrokerSettingsCollection ServiceBrokerSettingsItems
        {
            get { return ((ServiceBrokerSettingsCollection)(base["serviceBrokerSettings"])); }
        }
    }
}
