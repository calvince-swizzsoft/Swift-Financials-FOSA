using System.Configuration;

namespace SwiftFinancials.RecurringBatchPosting.Configuration
{
    public class RecurringBatchPostingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RecurringBatchPostingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RecurringBatchPostingSettingsElement)(element)).UniqueId;
        }

        public RecurringBatchPostingSettingsElement this[int index]
        {
            get { return (RecurringBatchPostingSettingsElement)BaseGet(index); }
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
    }
}
