using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeLevyAgg
{
    public class FixedDepositTypeLevy : Entity
    {
        public Guid FixedDepositTypeId { get; set; }

        public virtual FixedDepositType FixedDepositType { get; private set; }

        public Guid LevyId { get; set; }

        public virtual Levy Levy { get; private set; }
    }
}
