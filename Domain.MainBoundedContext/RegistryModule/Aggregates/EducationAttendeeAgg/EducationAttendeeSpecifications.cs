using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationAttendeeAgg
{
    public static class EducationAttendeeSpecifications
    {
        public static Specification<EducationAttendee> DefaultSpec()
        {
            Specification<EducationAttendee> specification = new TrueSpecification<EducationAttendee>();

            return specification;
        }

        public static ISpecification<EducationAttendee> EducationAttendeeWithEducationRegisterId(Guid educationRegisterId)
        {
            Specification<EducationAttendee> specification = new TrueSpecification<EducationAttendee>();

            if (educationRegisterId != null && educationRegisterId != Guid.Empty)
            {
                specification &= new DirectSpecification<EducationAttendee>(x => x.EducationRegisterId == educationRegisterId);
            }

            return specification;
        }

        public static ISpecification<EducationAttendee> EducationAttendeeWithCustomerId(Guid customerId, Guid postingPeriodId)
        {
            Specification<EducationAttendee> specification = new TrueSpecification<EducationAttendee>();

            if (customerId != null && customerId != Guid.Empty && postingPeriodId != null && postingPeriodId != Guid.Empty)
            {
                specification &= new DirectSpecification<EducationAttendee>(x => x.CustomerId == customerId && x.EducationRegister.PostingPeriodId == postingPeriodId);
            }

            return specification;
        }

        public static Specification<EducationAttendee> EducationAttendeeFullText(Guid educationRegisterId, string text)
        {
            Specification<EducationAttendee> specification = new DirectSpecification<EducationAttendee>(x => x.EducationRegisterId == educationRegisterId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<EducationAttendee>(c => c.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<EducationAttendee>(c => c.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<EducationAttendee>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<EducationAttendee>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<EducationAttendee>(c => c.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<EducationAttendee>(c => c.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<EducationAttendee>(c => c.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<EducationAttendee>(c => c.Customer.Address.MobileLine.Contains(text));

                var createdBySpec = new DirectSpecification<EducationAttendee>(c => c.CreatedBy.Contains(text));

                specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec | createdBySpec);
            }

            return specification;
        }
    }
}
