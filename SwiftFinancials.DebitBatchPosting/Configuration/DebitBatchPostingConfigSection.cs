using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.DebitBatchPosting.Configuration
{
    public class DebitBatchPostingConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("debitBatchPostingSettings")]
        public DebitBatchPostingSettingsCollection DebitBatchPostingSettingsItems
        {
            get { return ((DebitBatchPostingSettingsCollection)(base["debitBatchPostingSettings"])); }
        }
    }
}
