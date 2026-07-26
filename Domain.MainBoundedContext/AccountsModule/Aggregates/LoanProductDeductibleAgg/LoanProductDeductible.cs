using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDeductibleAgg
{
    public class LoanProductDeductible : Entity
    {
        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public string Description { get; set; }

        public virtual CustomerAccountType CustomerAccountType { get; set; }

        public virtual Charge Charge { get; set; }

        public bool NetOffInvestmentBalance { get; set; }

        public bool ComputeChargeOnTopUp { get; set; }

        
    }
}
