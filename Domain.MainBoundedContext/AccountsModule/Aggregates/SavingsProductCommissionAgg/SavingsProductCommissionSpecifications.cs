using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductCommissionAgg
{
    public static class SavingsProductCommissionSpecifications
    {
        public static Specification<SavingsProductCommission> DefaultSpec()
        {
            Specification<SavingsProductCommission> specification = new TrueSpecification<SavingsProductCommission>();

            return specification;
        }

        public static ISpecification<SavingsProductCommission> SavingsProductCommission(Guid savingsProductId, int savingsProductKnownChargeType)
        {
            Specification<SavingsProductCommission> specification = new TrueSpecification<SavingsProductCommission>();

            if (savingsProductId != null && savingsProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<SavingsProductCommission>(x => x.SavingsProductId == savingsProductId && x.KnownChargeType == savingsProductKnownChargeType);
            }

            return specification;
        }
    }
}
