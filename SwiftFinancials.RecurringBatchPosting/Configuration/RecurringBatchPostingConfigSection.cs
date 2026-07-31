using System.Configuration;

namespace SwiftFinancials.RecurringBatchPosting.Configuration
{
    public class RecurringBatchPostingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("recurringBatchPostingSettings")]
        public RecurringBatchPostingSettingsCollection RecurringBatchPostingSettingsItems
        {
            get { return ((RecurringBatchPostingSettingsCollection)(base["recurringBatchPostingSettings"])); }
        }
    }
}
