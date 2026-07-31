using System.Configuration;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Configuration
{
    public class TextDispatcherSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TextDispatcherSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TextDispatcherSettingsElement)(element)).UniqueId;
        }

        public TextDispatcherSettingsElement this[int index]
        {
            get { return (TextDispatcherSettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("queuePath", IsRequired = true)]
        public string QueuePath
        {
            get
            {
                return (string)base["queuePath"];
            }
        }

        [ConfigurationProperty("queueingJobCronExpression", IsRequired = true)]
        public string QueueingJobCronExpression
        {
            get
            {
                return (string)base["queueingJobCronExpression"];
            }
        }

        [ConfigurationProperty("queueReceivers", IsRequired = true)]
        public int QueueReceivers
        {
            get
            {
                return (int)base["queueReceivers"];
            }
        }

        [ConfigurationProperty("queueDaysCap", IsRequired = true)]
        public int QueueDaysCap
        {
            get
            {
                return (int)base["queueDaysCap"];
            }
        }

        [ConfigurationProperty("logEnabled", IsRequired = true)]
        public int LogEnabled
        {
            get
            {
                return ((int)(base["logEnabled"]));
            }
        }
    }
}
