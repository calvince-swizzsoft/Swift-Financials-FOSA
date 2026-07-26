using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeCommissionAgg
{
    public class WireTransferTypeCommission : Entity
    {
        public Guid WireTransferTypeId { get; set; }

        public virtual WireTransferType WireTransferType { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }
    }
}
