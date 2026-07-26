using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingEntryAgg
{
    public static class ConditionalLendingEntrySpecifications
    {
        public static Specification<ConditionalLendingEntry> DefaultSpec()
        {
            Specification<ConditionalLendingEntry> specification = new TrueSpecification<ConditionalLendingEntry>();

            return specification;
        }

        public static Specification<ConditionalLendingEntry> ConditionalLendingEntryWithConditionalLendingId(Guid conditionalLendingId, string text)
        {
            Specification<ConditionalLendingEntry> specification = new DirectSpecification<ConditionalLendingEntry>(c => c.ConditionalLendingId == conditionalLendingId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.Customer.Address.MobileLine.Contains(text));

                var createdBySpec = new DirectSpecification<ConditionalLendingEntry>(c => c.CreatedBy.Contains(text));

                specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<ConditionalLendingEntry> ConditionalLendingEntryWithCustomerIdAndConditionalLendingId(Guid customerId, Guid conditionalLendingId)
        {
            Specification<ConditionalLendingEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty && conditionalLendingId != null && conditionalLendingId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.CustomerId == customerId && c.ConditionalLendingId == conditionalLendingId);

                specification &= customerIdSpec;
            }

            return specification;
        }

        public static Specification<ConditionalLendingEntry> ConditionalLendingEntryWithCustomerIdAndLoanProductId(Guid customerId, Guid loanProductId)
        {
            Specification<ConditionalLendingEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.CustomerId == customerId && c.ConditionalLending.LoanProductId == loanProductId && !c.ConditionalLending.IsLocked);

                specification &= customerIdSpec;
            }

            return specification;
        }

        public static Specification<ConditionalLendingEntry> ConditionalLendingEntryWithCustomerId(Guid customerId)
        {
            Specification<ConditionalLendingEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<ConditionalLendingEntry>(c => c.CustomerId == customerId);

                specification &= customerIdSpec;
            }

            return specification;
        }
    }
}
