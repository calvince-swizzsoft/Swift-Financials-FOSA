using System.Configuration;

namespace SwiftFinancials.EmailAlertDispatcher.Configuration
{
    public class EmailDispatcherSettingsElement : ConfigurationElement
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

        [ConfigurationProperty("smtpHost", IsKey = false, IsRequired = true)]
        public string SmtpHost
        {
            get { return ((string)(base["smtpHost"])); }
            set { base["smtpHost"] = value; }
        }

        [ConfigurationProperty("smtpPort", IsKey = false, IsRequired = true)]
        public int SmtpPort
        {
            get { return ((int)(base["smtpPort"])); }
            set { base["smtpPort"] = value; }
        }

        [ConfigurationProperty("smtpEnableSsl", IsKey = false, IsRequired = true)]
        public int SmtpEnableSsl
        {
            get { return ((int)(base["smtpEnableSsl"])); }
            set { base["smtpEnableSsl"] = value; }
        }

        [ConfigurationProperty("smtpUsername", IsKey = false, IsRequired = true)]
        public string SmtpUsername
        {
            get { return ((string)(base["smtpUsername"])); }
            set { base["smtpUsername"] = value; }
        }

        [ConfigurationProperty("smtpPassword", IsKey = false, IsRequired = true)]
        public string SmtpPassword
        {
            get { return ((string)(base["smtpPassword"])); }
            set { base["smtpPassword"] = value; }
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
