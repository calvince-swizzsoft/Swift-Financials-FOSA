using System.Configuration;

namespace SwiftFinancials.CreditBatchPosting.Configuration
{
    public class CreditBatchPostingSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("uniqueId", IsKey = true, IsRequired = true)]
        public string UniqueId
        {
            get { return ((string)(base["uniqueId"])); }
            set { base["uniqueId"] = value; }
        }

        [ConfigurationProperty("queuePageSize", IsKey = false, IsRequired = true)]
        public int QueuePageSize
        {
            get { return ((int)(base["queuePageSize"])); }
            set { base["queuePageSize"] = value; }
        }

        [ConfigurationProperty("timeToBeReceived", IsKey = false, IsRequired = true)]
        public int TimeToBeReceived
        {
            get { return ((int)(base["timeToBeReceived"])); }
            set { base["timeToBeReceived"] = value; }
        }

        [ConfigurationProperty("enabled", IsKey = false, IsRequired = true)]
        public int Enabled
        {
            get { return ((int)(base["enabled"])); }
            set { base["enabled"] = value; }
        }
    }
}
