using System.Configuration;

namespace SwiftFinancials.ArrearsRecovery.Configuration
{
    public class ArrearsRecoverySettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ArrearsRecoverySettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ArrearsRecoverySettingsElement)(element)).UniqueId;
        }

        public ArrearsRecoverySettingsElement this[int index]
        {
            get { return (ArrearsRecoverySettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("arrearsRecoveryJobCronExpression", IsRequired = true)]
        public string ArrearsRecoveryJobCronExpression
        {
            get
            {
                return (string)base["arrearsRecoveryJobCronExpression"];
            }
        }
    }
}
