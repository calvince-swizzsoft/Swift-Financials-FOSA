using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeCommissionAgg
{
    public static class ChequeTypeCommissionSpecifications
    {
        public static Specification<ChequeTypeCommission> DefaultSpec()
        {
            Specification<ChequeTypeCommission> specification = new TrueSpecification<ChequeTypeCommission>();

            return specification;
        }

        public static ISpecification<ChequeTypeCommission> ChequeTypeCommissionWithChequeTypeId(Guid chequeTypeId)
        {
            Specification<ChequeTypeCommission> specification = new TrueSpecification<ChequeTypeCommission>();

            if (chequeTypeId != null && chequeTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<ChequeTypeCommission>(x => x.ChequeTypeId == chequeTypeId);
            }

            return specification;
        }
    }
}
