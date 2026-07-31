using System.Configuration;

namespace SwiftFinancials.InterestCapitalization.Configuration
{
    public class InterestCapitalizationSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new InterestCapitalizationSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InterestCapitalizationSettingsElement)(element)).UniqueId;
        }

        public InterestCapitalizationSettingsElement this[int index]
        {
            get { return (InterestCapitalizationSettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }
        
        [ConfigurationProperty("interestCapitalizationJobCronExpression", IsRequired = true)]
        public string InterestCapitalizationJobCronExpression
        {
            get
            {
                return (string)base["interestCapitalizationJobCronExpression"];
            }
        }
    }
}
