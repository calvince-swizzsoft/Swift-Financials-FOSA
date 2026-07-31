using System.Configuration;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Configuration
{
    public class TextDispatcherSettingsElement : ConfigurationElement
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

        [ConfigurationProperty("bulkTextUrl", IsKey = false, IsRequired = false)]
        public string BulkTextUrl
        {
            get { return ((string)(base["bulkTextUrl"])); }
            set { base["bulkTextUrl"] = value; }
        }

        [ConfigurationProperty("bulkTextUsername", IsKey = false, IsRequired = false)]
        public string BulkTextUsername
        {
            get { return ((string)(base["bulkTextUsername"])); }
            set { base["bulkTextUsername"] = value; }
        }

        [ConfigurationProperty("bulkTextPassword", IsKey = false, IsRequired = false)]
        public string BulkTextPassword
        {
            get { return ((string)(base["bulkTextPassword"])); }
            set { base["bulkTextPassword"] = value; }
        }

        [ConfigurationProperty("bulkTextSenderId", IsKey = false, IsRequired = false)]
        public string BulkTextSenderId
        {
            get { return ((string)(base["bulkTextSenderId"])); }
            set { base["bulkTextSenderId"] = value; }
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
