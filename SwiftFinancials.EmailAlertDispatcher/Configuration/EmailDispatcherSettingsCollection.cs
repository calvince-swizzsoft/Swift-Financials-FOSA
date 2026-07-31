using System.Configuration;

namespace SwiftFinancials.EmailAlertDispatcher.Configuration
{
    public class EmailDispatcherSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EmailDispatcherSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EmailDispatcherSettingsElement)(element)).UniqueId;
        }

        public EmailDispatcherSettingsElement this[int index]
        {
            get { return (EmailDispatcherSettingsElement)BaseGet(index); }
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
                return ((int)(base["queueDaysCap"]));
            }
        }

        [ConfigurationProperty("attachmentStagingFolder", IsRequired = true)]
        public string AttachmentStagingFolder
        {
            get
            {
                return (string)base["attachmentStagingFolder"];
            }
        }
    }
}
