using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchDiscrepancyAgg
{
    public class CreditBatchDiscrepancy : Entity
    {
        public Guid CreditBatchId { get; set; }

        public virtual CreditBatch CreditBatch { get; private set; }

        public string Column1 { get; set; }

        public string Column2 { get; set; }

        public string Column3 { get; set; }

        public string Column4 { get; set; }

        public string Column5 { get; set; }

        public string Column6 { get; set; }

        public string Column7 { get; set; }

        public string Column8 { get; set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        public string PostedBy { get; set; }

        public DateTime? PostedDate { get; set; }
    }
}
