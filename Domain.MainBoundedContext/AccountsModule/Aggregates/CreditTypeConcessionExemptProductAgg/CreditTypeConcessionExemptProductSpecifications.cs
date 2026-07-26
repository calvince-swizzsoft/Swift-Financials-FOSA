using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeConcessionExemptProductAgg
{
    public static class CreditTypeConcessionExemptProductSpecifications
    {
        public static Specification<CreditTypeConcessionExemptProduct> DefaultSpec()
        {
            Specification<CreditTypeConcessionExemptProduct> specification = new TrueSpecification<CreditTypeConcessionExemptProduct>();

            return specification;
        }

        public static ISpecification<CreditTypeConcessionExemptProduct> CreditTypeConcessionExemptProductWithCreditTypeId(Guid creditTypeId)
        {
            Specification<CreditTypeConcessionExemptProduct> specification = new TrueSpecification<CreditTypeConcessionExemptProduct>();

            if (creditTypeId != null && creditTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<CreditTypeConcessionExemptProduct>(x => x.CreditTypeId == creditTypeId);
            }

            return specification;
        }

        public static ISpecification<CreditTypeConcessionExemptProduct> CreditTypeConcessionExemptProductWithTargetProductId(Guid targetProductId)
        {
            Specification<CreditTypeConcessionExemptProduct> specification = new TrueSpecification<CreditTypeConcessionExemptProduct>();

            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<CreditTypeConcessionExemptProduct>(x => x.TargetProductId == targetProductId);
            }

            return specification;
        }
    }
}
