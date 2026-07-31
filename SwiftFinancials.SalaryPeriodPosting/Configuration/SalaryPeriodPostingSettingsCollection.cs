using System.Configuration;

namespace SwiftFinancials.SalaryPeriodPosting.Configuration
{
    public class SalaryPeriodPostingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SalaryPeriodPostingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SalaryPeriodPostingSettingsElement)(element)).UniqueId;
        }

        public SalaryPeriodPostingSettingsElement this[int index]
        {
            get { return (SalaryPeriodPostingSettingsElement)BaseGet(index); }
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
