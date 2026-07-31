using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.DebitBatchPosting.Configuration
{
    public class DebitBatchPostingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DebitBatchPostingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DebitBatchPostingSettingsElement)(element)).UniqueId;
        }

        public DebitBatchPostingSettingsElement this[int index]
        {
            get { return (DebitBatchPostingSettingsElement)BaseGet(index); }
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
