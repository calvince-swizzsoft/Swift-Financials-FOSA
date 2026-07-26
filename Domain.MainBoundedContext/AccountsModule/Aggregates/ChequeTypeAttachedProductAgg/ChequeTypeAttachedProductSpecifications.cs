using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAttachedProductAgg
{
    public static class ChequeTypeAttachedProductSpecifications
    {
        public static Specification<ChequeTypeAttachedProduct> DefaultSpec()
        {
            Specification<ChequeTypeAttachedProduct> specification = new TrueSpecification<ChequeTypeAttachedProduct>();

            return specification;
        }

        public static ISpecification<ChequeTypeAttachedProduct> ChequeTypeAttachedProductWithChequeTypeId(Guid chequeTypeId)
        {
            Specification<ChequeTypeAttachedProduct> specification = new TrueSpecification<ChequeTypeAttachedProduct>();

            if (chequeTypeId != null && chequeTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<ChequeTypeAttachedProduct>(x => x.ChequeTypeId == chequeTypeId);
            }

            return specification;
        }

        public static ISpecification<ChequeTypeAttachedProduct> ChequeTypeAttachedProductWithTargetProductId(Guid targetProductId)
        {
            Specification<ChequeTypeAttachedProduct> specification = new TrueSpecification<ChequeTypeAttachedProduct>();

            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<ChequeTypeAttachedProduct>(x => x.TargetProductId == targetProductId);
            }

            return specification;
        }
    }
}
