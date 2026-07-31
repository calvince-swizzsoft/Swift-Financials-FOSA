using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.InterestCapitalization.Configuration
{
    public class InterestCapitalizationConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("interestCapitalizationSettings")]
        public InterestCapitalizationSettingsCollection InterestCapitalizationSettingsItems
        {
            get { return ((InterestCapitalizationSettingsCollection)(base["interestCapitalizationSettings"])); }
        }
    }
}
