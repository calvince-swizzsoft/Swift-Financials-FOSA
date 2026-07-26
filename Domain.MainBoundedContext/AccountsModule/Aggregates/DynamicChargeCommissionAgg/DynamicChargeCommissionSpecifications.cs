using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeCommissionAgg
{
    public static class DynamicChargeCommissionSpecifications
    {
        public static Specification<DynamicChargeCommission> DefaultSpec()
        {
            Specification<DynamicChargeCommission> specification = new TrueSpecification<DynamicChargeCommission>();

            return specification;
        }

        public static ISpecification<DynamicChargeCommission> DynamicChargeCommissionWithDynamicChargeId(Guid dynamicChargeId)
        {
            Specification<DynamicChargeCommission> specification = new TrueSpecification<DynamicChargeCommission>();

            if (dynamicChargeId != null && dynamicChargeId != Guid.Empty)
            {
                specification &= new DirectSpecification<DynamicChargeCommission>(x => x.DynamicChargeId == dynamicChargeId);
            }

            return specification;
        }
    }
}
