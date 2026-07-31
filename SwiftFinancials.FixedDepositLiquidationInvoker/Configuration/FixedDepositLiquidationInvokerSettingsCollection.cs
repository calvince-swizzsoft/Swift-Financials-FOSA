using System.Configuration;

namespace SwiftFinancials.FixedDepositLiquidationInvoker.Configuration
{
    public class FixedDepositLiquidationInvokerSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FixedDepositLiquidationInvokerSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FixedDepositLiquidationInvokerSettingsElement)(element)).UniqueId;
        }

        public FixedDepositLiquidationInvokerSettingsElement this[int index]
        {
            get { return (FixedDepositLiquidationInvokerSettingsElement)BaseGet(index); }
        }


        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("fixedDepositLiquidationJobCronExpression", IsRequired = true)]
        public string FixedDepositLiquidationJobCronExpression
        {
            get
            {
                return (string)base["fixedDepositLiquidationJobCronExpression"];
            }
        }
    }
}
