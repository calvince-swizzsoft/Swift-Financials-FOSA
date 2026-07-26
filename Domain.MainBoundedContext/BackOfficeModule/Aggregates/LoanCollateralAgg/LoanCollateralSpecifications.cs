using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCollateralAgg
{
    public static class LoanCollateralSpecifications
    {
        public static Specification<LoanCollateral> DefaultSpec()
        {
            Specification<LoanCollateral> specification = new TrueSpecification<LoanCollateral>();

            return specification;
        }

        public static Specification<LoanCollateral> LoanCollateralWithCustomerId(Guid customerId)
        {
            Specification<LoanCollateral> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var loanCaseIdSpec = new DirectSpecification<LoanCollateral>(c => c.CustomerDocument.CustomerId == customerId);

                specification &= loanCaseIdSpec;
            }

            return specification;
        }

        public static Specification<LoanCollateral> LoanCollateralWithLoanCaseId(Guid loanCaseId)
        {
            Specification<LoanCollateral> specification = DefaultSpec();

            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                var loanCaseIdSpec = new DirectSpecification<LoanCollateral>(c => c.LoanCaseId == loanCaseId);

                specification &= loanCaseIdSpec;
            }

            return specification;
        }

        public static Specification<LoanCollateral> LoanCollateralWithCustomerIdAndLoanProductId(Guid customerId, Guid loanProductId)
        {
            Specification<LoanCollateral> specification = new TrueSpecification<LoanCollateral>();

            if (customerId != null && customerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                var loaneeCustomerIdAndLoanProductIdSpec = new DirectSpecification<LoanCollateral>(c => c.CustomerDocument.CustomerId == customerId && c.LoanCase.LoanProductId == loanProductId);

                specification &= loaneeCustomerIdAndLoanProductIdSpec;
            }

            return specification;
        }

        public static Specification<LoanCollateral> LoanCollateralWithCustomerIdAndLoanProductId(Guid customerId, Guid loanProductId, int status)
        {
            Specification<LoanCollateral> specification = new TrueSpecification<LoanCollateral>();

            if (customerId != null && customerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                var loaneeCustomerIdAndLoanProductIdSpec = new DirectSpecification<LoanCollateral>(c => c.CustomerDocument.CustomerId == customerId && c.LoanCase.LoanProductId == loanProductId && c.CustomerDocument.Collateral.Status == status);

                specification &= loaneeCustomerIdAndLoanProductIdSpec;
            }

            return specification;
        }
    }
}
