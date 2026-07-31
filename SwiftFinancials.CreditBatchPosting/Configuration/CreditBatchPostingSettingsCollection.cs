using System.Configuration;

namespace SwiftFinancials.CreditBatchPosting.Configuration
{
    public class CreditBatchPostingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CreditBatchPostingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CreditBatchPostingSettingsElement)(element)).UniqueId;
        }

        public CreditBatchPostingSettingsElement this[int index]
        {
            get { return (CreditBatchPostingSettingsElement)BaseGet(index); }
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
