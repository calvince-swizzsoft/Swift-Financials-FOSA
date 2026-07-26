using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAppraisalProductAgg
{
    public static class LoanProductAppraisalProductSpecifications
    {
        public static Specification<LoanProductAppraisalProduct> DefaultSpec()
        {
            Specification<LoanProductAppraisalProduct> specification = new TrueSpecification<LoanProductAppraisalProduct>();

            return specification;
        }

        public static ISpecification<LoanProductAppraisalProduct> LoanProductAppraisalProductWithLoanProductId(Guid loanProductId)
        {
            Specification<LoanProductAppraisalProduct> specification = new TrueSpecification<LoanProductAppraisalProduct>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanProductAppraisalProduct>(x => x.LoanProductId == loanProductId);
            }

            return specification;
        }
    }
}
