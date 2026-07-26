using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanPurposeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRequestAgg
{
    public class LoanRequest : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public Guid LoanPurposeId { get; set; }

        public virtual LoanPurpose LoanPurpose { get; private set; }

        public decimal AmountApplied { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string Reference { get; set; }

        public int LoanCaseNumber { get; set; }

        public string RegisteredBy { get; set; }

        public DateTime? RegisteredDate { get; set; }

        public string CancelledBy { get; set; }

        public DateTime? CancelledDate { get; set; }

        public byte Status { get; set; }
    }
}
