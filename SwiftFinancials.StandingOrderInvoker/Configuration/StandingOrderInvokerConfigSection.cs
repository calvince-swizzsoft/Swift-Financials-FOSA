using System.Configuration;

namespace SwiftFinancials.StandingOrderInvoker.Configuration
{
    public class StandingOrderInvokerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("standingOrderInvokerSettings")]
        public StandingOrderInvokerSettingsCollection StandingOrderInvokerSettingsItems
        {
            get { return ((StandingOrderInvokerSettingsCollection)(base["standingOrderInvokerSettings"])); }
        }
    }
}
