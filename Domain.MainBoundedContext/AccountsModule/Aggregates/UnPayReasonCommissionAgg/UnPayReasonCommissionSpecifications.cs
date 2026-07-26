using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonCommissionAgg
{
    public static class UnPayReasonCommissionSpecifications
    {
        public static Specification<UnPayReasonCommission> DefaultSpec()
        {
            Specification<UnPayReasonCommission> specification = new TrueSpecification<UnPayReasonCommission>();

            return specification;
        }

        public static ISpecification<UnPayReasonCommission> UnPayReasonCommissionWithUnPayReasonId(Guid unPayReasonId)
        {
            Specification<UnPayReasonCommission> specification = new TrueSpecification<UnPayReasonCommission>();

            if (unPayReasonId != null && unPayReasonId != Guid.Empty)
            {
                specification &= new DirectSpecification<UnPayReasonCommission>(x => x.UnPayReasonId == unPayReasonId);
            }

            return specification;
        }
    }
}
