using System.Configuration;

namespace SwiftFinancials.LoanDisbursementBatchPosting.Configuration
{
    public class LoanDisbursementBatchPostingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoanDisbursementBatchPostingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoanDisbursementBatchPostingSettingsElement)(element)).UniqueId;
        }

        public LoanDisbursementBatchPostingSettingsElement this[int index]
        {
            get { return (LoanDisbursementBatchPostingSettingsElement)BaseGet(index); }
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
