using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg
{
    public class PurchaseInvoiceLine : Domain.Seedwork.Entity
    {
        
        public Guid PurchaseInvoiceId { get; set; }

        public int PurchaseInvoiceNo { get; set; }
        public int Type { get; set; }

        public int No { get; set; }

        public Guid DebitChartOfAccountId { get; set; }

        public string Description { get; set; }

        public decimal UnitCost { get; set;}

        public int Quantity { get; set; }

        public Decimal Amount { get; set; }

    }
}
