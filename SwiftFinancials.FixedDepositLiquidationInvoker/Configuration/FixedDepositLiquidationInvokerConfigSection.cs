using System.Configuration;

namespace SwiftFinancials.FixedDepositLiquidationInvoker.Configuration
{
    public class FixedDepositLiquidationInvokerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("fixedDepositLiquidationInvokerSettings")]
        public FixedDepositLiquidationInvokerSettingsCollection FixedDepositLiquidationInvokerSettingsItems
        {
            get { return ((FixedDepositLiquidationInvokerSettingsCollection)(base["fixedDepositLiquidationInvokerSettings"])); }
        }
    }
}
