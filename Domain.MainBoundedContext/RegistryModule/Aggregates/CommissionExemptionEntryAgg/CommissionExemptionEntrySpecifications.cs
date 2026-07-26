using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionEntryAgg
{
    public static class CommissionExemptionEntrySpecifications
    {
        public static Specification<CommissionExemptionEntry> DefaultSpec()
        {
            Specification<CommissionExemptionEntry> specification = new TrueSpecification<CommissionExemptionEntry>();

            return specification;
        }

        public static Specification<CommissionExemptionEntry> CommissionExemptionEntryWithCommissionExemptionId(Guid commissionExemptionId, string text)
        {
            Specification<CommissionExemptionEntry> specification = new DirectSpecification<CommissionExemptionEntry>(c => c.CommissionExemptionId == commissionExemptionId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.Customer.Address.MobileLine.Contains(text));

                var createdBySpec = new DirectSpecification<CommissionExemptionEntry>(c => c.CreatedBy.Contains(text));

                specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<CommissionExemptionEntry> CommissionExemptionEntryWithCustomerIdAndCommissionExemptionId(Guid customerId, Guid commissionExemptionId)
        {
            Specification<CommissionExemptionEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty && commissionExemptionId != null && commissionExemptionId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.CustomerId == customerId && c.CommissionExemptionId == commissionExemptionId);

                specification &= customerIdSpec;
            }

            return specification;
        }

        public static Specification<CommissionExemptionEntry> CommissionExemptionEntryWithCustomerIdAndCommissionId(Guid customerId, Guid commissionId)
        {
            Specification<CommissionExemptionEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty && commissionId != null && commissionId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.CustomerId == customerId && c.CommissionExemption.CommissionId == commissionId && !c.CommissionExemption.IsLocked);

                specification &= customerIdSpec;
            }

            return specification;
        }

        public static Specification<CommissionExemptionEntry> CommissionExemptionEntryWithCustomerId(Guid customerId)
        {
            Specification<CommissionExemptionEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<CommissionExemptionEntry>(c => c.CustomerId == customerId);

                specification &= customerIdSpec;
            }

            return specification;
        }
    }
}
