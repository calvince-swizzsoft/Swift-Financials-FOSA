using System.Configuration;

namespace SwiftFinancials.SavingsProductLedgerFeeProcessor.Configuration
{
    public class SavingsProductLedgerFeeProcessingSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SavingsProductLedgerFeeProcessingSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SavingsProductLedgerFeeProcessingSettingsElement)(element)).UniqueId;
        }

        public SavingsProductLedgerFeeProcessingSettingsElement this[int index]
        {
            get { return (SavingsProductLedgerFeeProcessingSettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("savingsProductLedgerFeeProcessingJobCronExpression", IsRequired = true)]
        public string SavingsProductLedgerFeeProcessingJobCronExpression
        {
            get
            {
                return (string)base["savingsProductLedgerFeeProcessingJobCronExpression"];
            }
        }
    }
}
