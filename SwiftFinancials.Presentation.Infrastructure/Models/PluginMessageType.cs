using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public enum PluginMessageType
    {
        [Description("ERROR")]
        Error = 1,
        [Description("INFORMATION")]
        Information = 2,
        [Description("WARNING")]
        Warning = 3
    }
}
