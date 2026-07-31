using System.Configuration;

namespace SwiftFinancials.SavingsProductLedgerFeeProcessor.Configuration
{
    public class SavingsProductLedgerFeeProcessingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("savingsProductLedgerFeeProcessingSettings")]
        public SavingsProductLedgerFeeProcessingSettingsCollection SavingsProductLedgerFeeProcessingSettingsItems
        {
            get { return ((SavingsProductLedgerFeeProcessingSettingsCollection)(base["savingsProductLedgerFeeProcessingSettings"])); }
        }
    }
}
