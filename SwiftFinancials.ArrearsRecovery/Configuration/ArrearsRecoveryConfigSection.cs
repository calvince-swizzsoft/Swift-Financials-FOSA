using System.Configuration;

namespace SwiftFinancials.ArrearsRecovery.Configuration
{
    public class ArrearsRecoveryConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("arrearsRecoverySettings")]
        public ArrearsRecoverySettingsCollection ArrearsRecoverySettingsItems
        {
            get { return ((ArrearsRecoverySettingsCollection)(base["arrearsRecoverySettings"])); }
        }
    }
}
