using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxiliaryConditionAgg
{
    public class LoanProductAuxiliaryCondition : Entity
    {
        public Guid BaseLoanProductId { get; set; }

        public virtual LoanProduct BaseLoanProduct { get; private set; }

        public Guid TargetLoanProductId { get; set; }

        public virtual LoanProduct TargetLoanProduct { get; private set; }

        public byte Condition { get; set; }

        public double MaximumEligiblePercentage { get; set; }
    }
}
