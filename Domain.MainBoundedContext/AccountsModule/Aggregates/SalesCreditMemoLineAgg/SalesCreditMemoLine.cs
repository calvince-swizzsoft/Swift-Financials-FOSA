using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoLineAgg
{
    public class SalesCreditMemoLine : Entity
    {

        public Guid SalesCreditMemoId { get; set; }

        public int SalesCreditMemoNo { get; set; }
        public int Type { get; set; }

        public int No { get; set; }

        public Guid DebitChartOfAccountId { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public Decimal Amount { get; set; }

    }
}
