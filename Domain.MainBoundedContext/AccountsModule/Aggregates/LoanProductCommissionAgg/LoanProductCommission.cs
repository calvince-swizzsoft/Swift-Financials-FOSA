using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductCommissionAgg
{
    public class LoanProductCommission : Entity
    {
        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        public int KnownChargeType { get; set; }

        public byte ChargeBasisValue { get; set; }

        
    }
}
