using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductCommissionAgg
{
    public static class LoanProductCommissionSpecifications
    {
        public static Specification<LoanProductCommission> DefaultSpec()
        {
            Specification<LoanProductCommission> specification = new TrueSpecification<LoanProductCommission>();

            return specification;
        }

        public static ISpecification<LoanProductCommission> LoanProductCommission(Guid loanProductId, int loanProductKnownChargeType)
        {
            Specification<LoanProductCommission> specification = new TrueSpecification<LoanProductCommission>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanProductCommission>(x => x.LoanProductId == loanProductId && x.KnownChargeType == loanProductKnownChargeType);
            }

            return specification;
        }
    }
}
