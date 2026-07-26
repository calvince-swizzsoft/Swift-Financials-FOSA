using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceLineAgg
{
    public class SalesInvoiceLine : Entity
    {


        public Guid SalesInvoiceId { get; set; }

        public int SalesInvoiceNo { get; set; }
        public int Type { get; set; }

        public int No { get; set; }

        public Guid CreditChartOfAccountId { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public Decimal Amount { get; set; }


    }
}
