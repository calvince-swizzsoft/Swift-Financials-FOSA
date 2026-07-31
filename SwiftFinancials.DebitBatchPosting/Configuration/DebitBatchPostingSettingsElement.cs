using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.DebitBatchPosting.Configuration
{
    public class DebitBatchPostingSettingsElement : ConfigurationElement
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
