using System.Configuration;

namespace SwiftFinancials.LoanDisbursementBatchPosting.Configuration
{
    public class LoanDisbursementBatchPostingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("loanDisbursementBatchPostingSettings")]
        public LoanDisbursementBatchPostingSettingsCollection LoanDisbursementBatchPostingSettingsItems
        {
            get { return ((LoanDisbursementBatchPostingSettingsCollection)(base["loanDisbursementBatchPostingSettings"])); }
        }
    }
}
