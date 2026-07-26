using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeGraduatedScaleAgg
{
    public class FixedDepositTypeGraduatedScale : Entity
    {
        public Guid FixedDepositTypeId { get; set; }

        public virtual FixedDepositType FixedDepositType { get; private set; }

        public virtual Range Range { get; set; }

        public double Percentage { get; set; }
    }
}
