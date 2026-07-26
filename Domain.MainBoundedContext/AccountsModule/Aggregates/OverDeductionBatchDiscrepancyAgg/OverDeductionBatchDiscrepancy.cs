using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchDiscrepancyAgg
{
    public class OverDeductionBatchDiscrepancy : Entity
    {
        public Guid OverDeductionBatchId { get; set; }

        public virtual OverDeductionBatch OverDeductionBatch { get; private set; }

        public string Column1 { get; set; }

        public string Column2 { get; set; }

        public string Column3 { get; set; }

        public string Column4 { get; set; }

        public string Column5 { get; set; }

        public string Column6 { get; set; }

        public string Column7 { get; set; }

        public string Column8 { get; set; }

        public string Column9 { get; set; }

        public string Column10 { get; set; }

        public string Column11 { get; set; }

        public string Column12 { get; set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        public string PostedBy { get; set; }

        public DateTime? PostedDate { get; set; }
    }
}
