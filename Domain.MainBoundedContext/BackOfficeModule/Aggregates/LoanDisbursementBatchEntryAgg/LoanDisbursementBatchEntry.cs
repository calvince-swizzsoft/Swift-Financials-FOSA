using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg
{
    public class LoanDisbursementBatchEntry : Entity
    {
        public Guid LoanDisbursementBatchId { get; set; }

        public virtual LoanDisbursementBatch LoanDisbursementBatch { get; private set; }

        public Guid LoanCaseId { get; set; }

        public virtual LoanCase LoanCase { get; private set; }

        public string Reference { get; set; }

        public byte Status { get; set; }
    }
}
