using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxilliaryAppraisalFactorAgg
{
    public class LoanProductAuxilliaryAppraisalFactor : Entity
    {
        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public virtual Range Range { get; set; }

        public double LoaneeMultiplier { get; set; }

        public double GuarantorMultiplier { get; set; }
    }
}
