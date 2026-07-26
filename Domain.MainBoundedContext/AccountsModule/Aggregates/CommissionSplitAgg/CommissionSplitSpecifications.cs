using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionSplitAgg
{
    public static class CommissionSplitSpecifications
    {
        public static Specification<CommissionSplit> DefaultSpec()
        {
            Specification<CommissionSplit> specification = new TrueSpecification<CommissionSplit>();

            return specification;
        }

        public static ISpecification<CommissionSplit> CommissionSplitWithCommissionId(Guid commissionId)
        {
            Specification<CommissionSplit> specification = new TrueSpecification<CommissionSplit>();

            if (commissionId != null && commissionId != Guid.Empty)
            {
                specification &= new DirectSpecification<CommissionSplit>(x => x.CommissionId == commissionId);
            }

            return specification;
        }
    }
}
