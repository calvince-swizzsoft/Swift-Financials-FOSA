using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductExemptionAgg
{
    public static class SavingsProductExemptionSpecifications
    {
        public static Specification<SavingsProductExemption> DefaultSpec()
        {
            Specification<SavingsProductExemption> specification = new TrueSpecification<SavingsProductExemption>();

            return specification;
        }

        public static ISpecification<SavingsProductExemption> SavingsProductExemptionWithSavingsProductId(Guid savingsProductId)
        {
            Specification<SavingsProductExemption> specification = new TrueSpecification<SavingsProductExemption>();

            if (savingsProductId != null && savingsProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<SavingsProductExemption>(x => x.SavingsProductId == savingsProductId);
            }

            return specification;
        }
    }
}
