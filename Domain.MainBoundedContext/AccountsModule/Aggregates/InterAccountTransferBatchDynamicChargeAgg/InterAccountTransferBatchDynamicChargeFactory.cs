using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchDynamicChargeAgg
{
    public static class InterAccountTransferBatchDynamicChargeFactory
    {
        public static InterAccountTransferBatchDynamicCharge CreateInterAccountTransferBatchDynamicCharge(Guid interAccountTransferBatchId, Guid dynamicChargeId)
        {
            var interAccountTransferBatchDynamicCharge = new InterAccountTransferBatchDynamicCharge();

            interAccountTransferBatchDynamicCharge.GenerateNewIdentity();

            interAccountTransferBatchDynamicCharge.InterAccountTransferBatchId = interAccountTransferBatchId;

            interAccountTransferBatchDynamicCharge.DynamicChargeId = dynamicChargeId;

            interAccountTransferBatchDynamicCharge.CreatedDate = DateTime.Now;

            return interAccountTransferBatchDynamicCharge;
        }
    }
}
