using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionLevyAgg
{
    public static class CommissionLevySpecifications
    {
        public static Specification<CommissionLevy> DefaultSpec()
        {
            Specification<CommissionLevy> specification = new TrueSpecification<CommissionLevy>();

            return specification;
        }

        public static ISpecification<CommissionLevy> CommissionLevyWithCommissionId(Guid commissionId)
        {
            Specification<CommissionLevy> specification = new TrueSpecification<CommissionLevy>();

            if (commissionId != null && commissionId != Guid.Empty)
            {
                specification &= new DirectSpecification<CommissionLevy>(x => x.CommissionId == commissionId);
            }

            return specification;
        }
    }
}
