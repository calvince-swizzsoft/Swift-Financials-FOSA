using System.Configuration;

namespace SwiftFinancials.GuarantorReleasing.Configuration
{
    public class GuarantorReleasingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new GuarantorReleasingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((GuarantorReleasingSettingsElement)(element)).UniqueId;
        }

        public GuarantorReleasingSettingsElement this[int index]
        {
            get { return (GuarantorReleasingSettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("guarantorReleasingJobCronExpression", IsRequired = true)]
        public string GuarantorReleasingJobCronExpression
        {
            get
            {
                return (string)base["guarantorReleasingJobCronExpression"];
            }
        }
    }
}
