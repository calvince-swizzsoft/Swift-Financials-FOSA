using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAttachedProductAgg
{
    public static class CreditTypeAttachedProductSpecifications
    {
        public static Specification<CreditTypeAttachedProduct> DefaultSpec()
        {
            Specification<CreditTypeAttachedProduct> specification = new TrueSpecification<CreditTypeAttachedProduct>();

            return specification;
        }

        public static ISpecification<CreditTypeAttachedProduct> CreditTypeAttachedProductWithCreditTypeId(Guid creditTypeId)
        {
            Specification<CreditTypeAttachedProduct> specification = new TrueSpecification<CreditTypeAttachedProduct>();

            if (creditTypeId != null && creditTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<CreditTypeAttachedProduct>(x => x.CreditTypeId == creditTypeId);
            }

            return specification;
        }

        public static ISpecification<CreditTypeAttachedProduct> CreditTypeAttachedProductWithTargetProductId(Guid targetProductId)
        {
            Specification<CreditTypeAttachedProduct> specification = new TrueSpecification<CreditTypeAttachedProduct>();

            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<CreditTypeAttachedProduct>(x => x.TargetProductId == targetProductId);
            }

            return specification;
        }
    }
}
