using System.Configuration;

namespace SwiftFinancials.GuarantorReleasing.Configuration
{
    public class GuarantorReleasingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("guarantorReleasingSettings")]
        public GuarantorReleasingSettingsCollection GuarantorReleasingSettingsItems
        {
            get { return ((GuarantorReleasingSettingsCollection)(base["guarantorReleasingSettings"])); }
        }
    }
}
