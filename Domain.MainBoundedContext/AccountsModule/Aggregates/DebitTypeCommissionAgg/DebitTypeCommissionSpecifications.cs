using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeCommissionAgg
{
    public static class DebitTypeCommissionSpecifications
    {
        public static Specification<DebitTypeCommission> DefaultSpec()
        {
            Specification<DebitTypeCommission> specification = new TrueSpecification<DebitTypeCommission>();

            return specification;
        }

        public static ISpecification<DebitTypeCommission> DebitTypeCommissionWithDebitTypeId(Guid debitTypeId)
        {
            Specification<DebitTypeCommission> specification = new TrueSpecification<DebitTypeCommission>();

            if (debitTypeId != null && debitTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<DebitTypeCommission>(x => x.DebitTypeId == debitTypeId);
            }

            return specification;
        }
    }
}
