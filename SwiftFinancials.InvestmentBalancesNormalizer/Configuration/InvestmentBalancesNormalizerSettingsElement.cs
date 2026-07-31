using System.Configuration;

namespace SwiftFinancials.InvestmentBalancesNormalizer.Configuration
{
    public class InvestmentBalancesNormalizerSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("uniqueId", IsKey = true, IsRequired = true)]
        public string UniqueId
        {
            get { return ((string)(base["uniqueId"])); }
            set { base["uniqueId"] = value; }
        }

        [ConfigurationProperty("normalizationSets", IsKey = false, IsRequired = true)]
        public string NormalizationSets
        {
            get { return ((string)(base["normalizationSets"])); }
            set { base["normalizationSets"] = value; }
        }

        [ConfigurationProperty("enforceCeiling", IsKey = false, IsRequired = true)]
        public int EnforceCeiling
        {
            get { return ((int)(base["enforceCeiling"])); }
            set { base["enforceCeiling"] = value; }
        }

        [ConfigurationProperty("enabled", IsKey = false, IsRequired = true)]
        public int Enabled
        {
            get { return ((int)(base["enabled"])); }
            set { base["enabled"] = value; }
        }
    }
}
