using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg
{
    public class LoanGuarantor : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid? LoaneeCustomerId { get; set; }

        public virtual Customer LoaneeCustomer { get; private set; }

        public Guid? LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public Guid? LoanCaseId { get; set; }

        public virtual LoanCase LoanCase { get; private set; }

        public byte Status { get; set; }

        public decimal TotalShares { get; set; }

        public decimal CommittedShares { get; set; }

        public decimal AmountGuaranteed { get; set; }

        public decimal AmountPledged { get; set; }

        public double AppraisalFactor { get; set; }
    }
}
