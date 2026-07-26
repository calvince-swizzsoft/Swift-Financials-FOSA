using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanCycleAgg
{
    public static class LoanCycleSpecifications
    {
        public static Specification<LoanCycle> DefaultSpec()
        {
            Specification<LoanCycle> specification = new TrueSpecification<LoanCycle>();

            return specification;
        }

        public static ISpecification<LoanCycle> LoanCycleWithLoanProductId(Guid loanProductId)
        {
            Specification<LoanCycle> specification = new TrueSpecification<LoanCycle>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanCycle>(x => x.LoanProductId == loanProductId);
            }

            return specification;
        }
    }
}
