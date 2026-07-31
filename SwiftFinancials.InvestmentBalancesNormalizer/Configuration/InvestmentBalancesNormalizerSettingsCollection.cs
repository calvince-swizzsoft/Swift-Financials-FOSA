using System.Configuration;

namespace SwiftFinancials.InvestmentBalancesNormalizer.Configuration
{
    public class InvestmentBalancesNormalizerSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new InvestmentBalancesNormalizerSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InvestmentBalancesNormalizerSettingsElement)(element)).UniqueId;
        }

        public InvestmentBalancesNormalizerSettingsElement this[int index]
        {
            get { return (InvestmentBalancesNormalizerSettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("normalizationJobCronExpression", IsRequired = true)]
        public string NormalizationJobCronExpression
        {
            get
            {
                return (string)base["normalizationJobCronExpression"];
            }
        }

        [ConfigurationProperty("poolingJobCronExpression", IsRequired = true)]
        public string PoolingJobCronExpression
        {
            get
            {
                return (string)base["poolingJobCronExpression"];
            }
        }
    }
}
