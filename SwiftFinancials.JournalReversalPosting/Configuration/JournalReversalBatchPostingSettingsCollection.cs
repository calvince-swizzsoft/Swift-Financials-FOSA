using System.Configuration;

namespace SwiftFinancials.JournalReversalBatchPosting.Configuration
{
    public class JournalReversalBatchPostingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JournalReversalBatchPostingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JournalReversalBatchPostingSettingsElement)(element)).UniqueId;
        }

        public JournalReversalBatchPostingSettingsElement this[int index]
        {
            get { return (JournalReversalBatchPostingSettingsElement)BaseGet(index); }
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
