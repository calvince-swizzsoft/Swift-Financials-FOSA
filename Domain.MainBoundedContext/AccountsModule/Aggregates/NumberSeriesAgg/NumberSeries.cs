using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.NumberSeriesAgg
{
    public class NumberSeries : Domain.Seedwork.Entity
    {
        public string Code { get; set; }          // e.g. "PAYVOUCHER", "PURCHASEINV"
        public string Prefix { get; set; }        // e.g. "PV", "PI"
        public int LastUsedNumber { get; set; }   // e.g. 123
        public int Padding { get; set; } = 5;
    }
}
