using System.Configuration;

namespace SwiftFinancials.SalaryPeriodPosting.Configuration
{
    public class SalaryPeriodPostingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("salaryPeriodPostingSettings")]
        public SalaryPeriodPostingSettingsCollection SalaryPeriodPostingSettingsItems
        {
            get { return ((SalaryPeriodPostingSettingsCollection)(base["salaryPeriodPostingSettings"])); }
        }
    }
}
