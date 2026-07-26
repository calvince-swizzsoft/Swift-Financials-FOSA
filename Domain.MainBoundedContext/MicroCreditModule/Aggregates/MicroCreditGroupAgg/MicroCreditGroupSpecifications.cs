using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupAgg
{
    public static class MicroCreditGroupSpecifications
    {
        public static Specification<MicroCreditGroup> DefaultSpec()
        {
            Specification<MicroCreditGroup> specification = new TrueSpecification<MicroCreditGroup>();

            return specification;
        }

        public static Specification<MicroCreditGroup> MicroCreditGroupFullText(string text)
        {
            Specification<MicroCreditGroup> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var purposeSpec = new DirectSpecification<MicroCreditGroup>(c => c.Purpose.Contains(text));
                var activitiesSpec = new DirectSpecification<MicroCreditGroup>(c => c.Activities.Contains(text));

                var descriptionSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.NonIndividual.Description.Contains(text));
                var personalIdentificationNumberSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.PersonalIdentificationNumber.Contains(text));
                var registrationNumberSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.NonIndividual.RegistrationNumber.Contains(text));

                var reference1Spec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Reference3.Contains(text));

                var addressEmailSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.Email.Contains(text));
                var addressCitySpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.City.Contains(text));
                var addressPostalCodeSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1Spec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2Spec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.AddressLine2.Contains(text));
                var addressStreetSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<MicroCreditGroup>(c => c.Customer.Address.MobileLine.Contains(text));

                var remarksSpec = new DirectSpecification<MicroCreditGroup>(c => c.Remarks.Contains(text));

                specification &= (purposeSpec | activitiesSpec
                    | descriptionSpec | personalIdentificationNumberSpec | registrationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                    | addressEmailSpec | addressCitySpec | addressPostalCodeSpec | addressAddressLine1Spec | addressAddressLine2Spec | addressStreetSpec | addressLandLineSpec | addressMobileLineSpec
                    | remarksSpec);
            }

            return specification;
        }

        public static ISpecification<MicroCreditGroup> MicroCreditGroupWithCustomerId(Guid customerId)
        {
            Specification<MicroCreditGroup> specification = new DirectSpecification<MicroCreditGroup>(x => x.CustomerId == customerId);

            return specification;
        }
    }
}
