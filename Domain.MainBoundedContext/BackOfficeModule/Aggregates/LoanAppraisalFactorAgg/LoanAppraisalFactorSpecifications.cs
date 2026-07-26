using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanAppraisalFactorAgg
{
    public static class LoanAppraisalFactorSpecifications
    {
        public static Specification<LoanAppraisalFactor> DefaultSpec()
        {
            Specification<LoanAppraisalFactor> specification = new TrueSpecification<LoanAppraisalFactor>();

            return specification;
        }

        public static Specification<LoanAppraisalFactor> LoanAppraisalFactorWithLoanCaseId(Guid loanCaseId)
        {
            Specification<LoanAppraisalFactor> specification = DefaultSpec();

            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                var loanCaseIdSpec = new DirectSpecification<LoanAppraisalFactor>(c => c.LoanCaseId == loanCaseId);

                specification &= loanCaseIdSpec;
            }

            return specification;
        }
    }
}
