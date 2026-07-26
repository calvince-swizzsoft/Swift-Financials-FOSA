using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeCommissionAgg
{
    public static class CreditTypeCommissionSpecifications
    {
        public static Specification<CreditTypeCommission> DefaultSpec()
        {
            Specification<CreditTypeCommission> specification = new TrueSpecification<CreditTypeCommission>();

            return specification;
        }

        public static ISpecification<CreditTypeCommission> CreditTypeCommissionWithCreditTypeId(Guid creditTypeId)
        {
            Specification<CreditTypeCommission> specification = new TrueSpecification<CreditTypeCommission>();

            if (creditTypeId != null && creditTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<CreditTypeCommission>(x => x.CreditTypeId == creditTypeId);
            }

            return specification;
        }
    }
}
