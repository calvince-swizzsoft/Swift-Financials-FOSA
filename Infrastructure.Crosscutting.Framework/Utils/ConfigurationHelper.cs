using Infrastructure.Crosscutting.Framework.Configuration;
using System.Configuration;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public static class ConfigurationHelper
    {
        public static ServiceBrokerConfigSection GetServiceBrokerConfiguration()
        {
            return (ServiceBrokerConfigSection)ConfigurationManager.GetSection("serviceBrokerConfiguration");
        }

        public static ServiceBrokerSettingsElement GetServiceBrokerConfigurationSettings(ServiceHeader serviceHeader)
        {
            ServiceBrokerSettingsElement serviceBrokerSettingsElement = null;

            var serviceBrokerConfigSection = GetServiceBrokerConfiguration();

            foreach (var settingsItem in serviceBrokerConfigSection.ServiceBrokerSettingsItems)
            {
                if (((ServiceBrokerSettingsElement)settingsItem).UniqueId == "SwiftFin_Dev")
                {
                    serviceBrokerSettingsElement = (ServiceBrokerSettingsElement)settingsItem;

                    break;
                }
            }

            return serviceBrokerSettingsElement;
        }
    }
}
