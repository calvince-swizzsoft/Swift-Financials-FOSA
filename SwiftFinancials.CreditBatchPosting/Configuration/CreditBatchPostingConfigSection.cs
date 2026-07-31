using System.Configuration;

namespace SwiftFinancials.CreditBatchPosting.Configuration
{
    public class CreditBatchPostingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("creditBatchPostingSettings")]
        public CreditBatchPostingSettingsCollection CreditBatchPostingSettingsItems
        {
            get { return ((CreditBatchPostingSettingsCollection)(base["creditBatchPostingSettings"])); }
        }
    }
}
