using System.Configuration;

namespace SwiftFinancials.StandingOrderInvoker.Configuration
{
    public class StandingOrderInvokerSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("uniqueId", IsKey = true, IsRequired = true)]
        public string UniqueId
        {
            get { return ((string)(base["uniqueId"])); }
            set { base["uniqueId"] = value; }
        }

        [ConfigurationProperty("targetDateOption", IsKey = false, IsRequired = true)]
        public int TargetDateOption
        {
            get { return ((int)(base["targetDateOption"])); }
            set { base["targetDateOption"] = value; }
        }

        [ConfigurationProperty("maximumStandingOrderExecuteAttemptCount", IsKey = false, IsRequired = true)]
        public int MaximumStandingOrderExecuteAttemptCount
        {
            get { return ((int)(base["maximumStandingOrderExecuteAttemptCount"])); }
            set { base["maximumStandingOrderExecuteAttemptCount"] = value; }
        }

        [ConfigurationProperty("queuePageSize", IsKey = false, IsRequired = true)]
        public int QueuePageSize
        {
            get { return ((int)(base["queuePageSize"])); }
            set { base["queuePageSize"] = value; }
        }

        [ConfigurationProperty("enabled", IsKey = false, IsRequired = true)]
        public int Enabled
        {
            get { return ((int)(base["enabled"])); }
            set { base["enabled"] = value; }
        }
    }
}
