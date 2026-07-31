using System.Configuration;

namespace SwiftFinancials.JournalReversalBatchPosting.Configuration
{
    public class JournalReversalBatchPostingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("journalReversalBatchPostingSettings")]
        public JournalReversalBatchPostingSettingsCollection JournalReversalBatchPostingSettingsItems
        {
            get { return ((JournalReversalBatchPostingSettingsCollection)(base["journalReversalBatchPostingSettings"])); }
        }
    }
}
