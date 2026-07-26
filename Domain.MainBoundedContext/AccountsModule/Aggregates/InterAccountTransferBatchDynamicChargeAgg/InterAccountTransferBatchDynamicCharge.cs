using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchDynamicChargeAgg
{
    public class InterAccountTransferBatchDynamicCharge : Entity
    {
        public Guid InterAccountTransferBatchId { get; set; }

        public virtual InterAccountTransferBatch InterAccountTransferBatch { get; private set; }

        public Guid DynamicChargeId { get; set; }

        public virtual DynamicCharge DynamicCharge { get; private set; }

        
    }
}
