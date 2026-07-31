using System.Configuration;

namespace SwiftFinancials.InvestmentBalancesNormalizer.Configuration
{
    public class InvestmentBalancesNormalizerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("investmentBalancesNormalizerSettings")]
        public InvestmentBalancesNormalizerSettingsCollection InvestmentBalancesNormalizerSettingsItems
        {
            get { return ((InvestmentBalancesNormalizerSettingsCollection)(base["investmentBalancesNormalizerSettings"])); }
        }
    }
}
