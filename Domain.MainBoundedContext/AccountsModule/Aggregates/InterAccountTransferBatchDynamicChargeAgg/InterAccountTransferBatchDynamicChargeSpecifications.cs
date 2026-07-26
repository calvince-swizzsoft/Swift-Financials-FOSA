using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchDynamicChargeAgg
{
    public static class InterAccountTransferBatchDynamicChargeSpecifications
    {
        public static Specification<InterAccountTransferBatchDynamicCharge> DefaultSpec()
        {
            Specification<InterAccountTransferBatchDynamicCharge> specification = new TrueSpecification<InterAccountTransferBatchDynamicCharge>();

            return specification;
        }

        public static ISpecification<InterAccountTransferBatchDynamicCharge> InterAccountTransferBatchDynamicChargeWithInterAccountTransferBatchId(Guid interAccountTransferBatchId)
        {
            Specification<InterAccountTransferBatchDynamicCharge> specification = new TrueSpecification<InterAccountTransferBatchDynamicCharge>();

            if (interAccountTransferBatchId != null && interAccountTransferBatchId != Guid.Empty)
            {
                specification &= new DirectSpecification<InterAccountTransferBatchDynamicCharge>(x => x.InterAccountTransferBatchId == interAccountTransferBatchId);
            }

            return specification;
        }
    }
}
