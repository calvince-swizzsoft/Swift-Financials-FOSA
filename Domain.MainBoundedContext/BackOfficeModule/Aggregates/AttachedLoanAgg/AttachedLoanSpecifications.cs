using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.AttachedLoanAgg
{
    public static class AttachedLoanSpecifications
    {
        public static Specification<AttachedLoan> DefaultSpec()
        {
            Specification<AttachedLoan> specification = new TrueSpecification<AttachedLoan>();

            return specification;
        }

        public static Specification<AttachedLoan> AttachedLoanWithLoanCaseId(Guid loanCaseId)
        {
            Specification<AttachedLoan> specification =
                new DirectSpecification<AttachedLoan>(c => c.LoanCaseId == loanCaseId);

            return specification;
        }
    }
}
