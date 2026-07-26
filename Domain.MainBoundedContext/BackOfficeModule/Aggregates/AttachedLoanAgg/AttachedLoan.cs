using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.AttachedLoanAgg
{
    public class AttachedLoan : Entity
    {
        public Guid LoanCaseId { get; set; }

        public virtual LoanCase LoanCase { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public decimal PrincipalBalance { get; set; }

        public decimal InterestBalance { get; set; }

        public decimal CarryForwardsBalance { get; set; }

        public decimal ClearanceCharges { get; set; }
    }
}
