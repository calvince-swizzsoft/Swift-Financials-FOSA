using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg
{
    public static class LoanGuarantorSpecifications
    {
        public static Specification<LoanGuarantor> DefaultSpec()
        {
            Specification<LoanGuarantor> specification = new TrueSpecification<LoanGuarantor>();

            return specification;
        }

        public static Specification<LoanGuarantor> LoanGuarantorWithCustomerId(Guid customerId)
        {
            Specification<LoanGuarantor> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var loanCaseIdSpec = new DirectSpecification<LoanGuarantor>(c => c.CustomerId == customerId);

                specification &= loanCaseIdSpec;
            }

            return specification;
        }

        public static Specification<LoanGuarantor> LoanGuarantorWithCustomerIdAndText(Guid customerId, string text)
        {
            Specification<LoanGuarantor> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var loanCaseIdSpec = new DirectSpecification<LoanGuarantor>(c => c.CustomerId == customerId);

                specification &= loanCaseIdSpec;
            }

            if (!String.IsNullOrWhiteSpace(text))
            {
                var loaneeFirstNameSpec = new DirectSpecification<LoanGuarantor>(c => c.LoaneeCustomer.Individual.FirstName.Contains(text));

                var loaneeLastNameSpec = new DirectSpecification<LoanGuarantor>(c => c.LoaneeCustomer.Individual.LastName.Contains(text));

                var loanProductSpec = new DirectSpecification<LoanGuarantor>(c => c.LoanProduct.Description.Contains(text));

                specification &= loaneeFirstNameSpec | loaneeLastNameSpec | loanProductSpec;
            }

            return specification;
        }

        public static Specification<LoanGuarantor> LoanGuarantorWithLoanCaseId(Guid loanCaseId)
        {
            Specification<LoanGuarantor> specification = DefaultSpec();

            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                var loanCaseIdSpec = new DirectSpecification<LoanGuarantor>(c => c.LoanCaseId == loanCaseId);

                specification &= loanCaseIdSpec;
            }

            return specification;
        }

        public static Specification<LoanGuarantor> LoanGuarantorWithLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId)
        {
            Specification<LoanGuarantor> specification = new TrueSpecification<LoanGuarantor>();

            if (loaneeCustomerId != null && loaneeCustomerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                var loaneeCustomerIdAndLoanProductIdSpec = new DirectSpecification<LoanGuarantor>(c => c.LoaneeCustomerId == loaneeCustomerId && c.LoanProductId == loanProductId);

                specification &= loaneeCustomerIdAndLoanProductIdSpec;
            }

            return specification;
        }

        public static Specification<LoanGuarantor> LoanGuarantorWithLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId, int status)
        {
            Specification<LoanGuarantor> specification = new TrueSpecification<LoanGuarantor>();

            if (loaneeCustomerId != null && loaneeCustomerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                var loaneeCustomerIdAndLoanProductIdSpec = new DirectSpecification<LoanGuarantor>(c => c.LoaneeCustomerId == loaneeCustomerId && c.LoanProductId == loanProductId && c.Status == status);

                specification &= loaneeCustomerIdAndLoanProductIdSpec;
            }

            return specification;
        }
        public static Specification<LoanGuarantor> LoanGuarantorFullText(string text)
        {
            Specification<LoanGuarantor> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<LoanGuarantor>(c => c.Customer.Individual.FirstName.Contains(text));

                var lastNameSpec = new DirectSpecification<LoanGuarantor>(c => c.Customer.Individual.LastName.Contains(text));

                var loanProductSpec = new DirectSpecification<LoanGuarantor>(c => c.LoanProduct.Description.Contains(text));

                specification &= firstNameSpec | lastNameSpec | loanProductSpec;
            }

            return specification;
        }
    }
}
