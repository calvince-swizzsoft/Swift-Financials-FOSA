using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoLineAgg
{
    public class PurchaseCreditMemoLine : Domain.Seedwork.Entity
    {

        public Guid PurchaseCreditMemoId { get; set; }

        public int PurchaseCreditMemoNo { get; set; }
        public int Type { get; set; }

        public int No { get; set; }

        public Guid CreditChartOfAccountId { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public int UnitCost { get; set;}

        public Decimal Amount { get; set; }

    }
}
