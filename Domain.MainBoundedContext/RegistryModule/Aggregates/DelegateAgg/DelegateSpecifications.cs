using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg
{
    public static class DelegateSpecifications
    {
        public static Specification<Delegate> DefaultSpec()
        {
            Specification<Delegate> specification = new TrueSpecification<Delegate>();

            return specification;
        }

        public static ISpecification<Delegate> DelegateWithCustomerId(Guid customerId)
        {
            Specification<Delegate> specification = new DirectSpecification<Delegate>(x => x.CustomerId == customerId);

            return specification;
        }

        public static Specification<Delegate> DelegateFullText(string text)
        {
            Specification<Delegate> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var membershipNumberSpec = new DirectSpecification<Delegate>(c => c.Customer.SerialNumber == number);

                    var payrollNumbersSpec = new DirectSpecification<Delegate>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<Delegate>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));

                    var addressLandLineSpec = new DirectSpecification<Delegate>(c => c.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<Delegate>(c => c.Customer.Address.MobileLine.Contains(text));

                    specification &= (membershipNumberSpec | payrollNumbersSpec | identificationNumberSpec | addressLandLineSpec | addressMobileLineSpec);
                }
                else
                {
                    var firstNameSpec = new DirectSpecification<Delegate>(c => c.Customer.Individual.FirstName.Contains(text));
                    var lastNameSpec = new DirectSpecification<Delegate>(c => c.Customer.Individual.LastName.Contains(text));
                    var payrollNumbersSpec = new DirectSpecification<Delegate>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<Delegate>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                    var referenceSpec = new DirectSpecification<Delegate>(c => c.Customer.Reference1.Contains(text));

                    var addressEmail = new DirectSpecification<Delegate>(c => c.Customer.Address.Email.Contains(text));
                    var addressCity = new DirectSpecification<Delegate>(c => c.Customer.Address.City.Contains(text));
                    var addressPostalCode = new DirectSpecification<Delegate>(c => c.Customer.Address.PostalCode.Contains(text));
                    var addressAddressLine1 = new DirectSpecification<Delegate>(c => c.Customer.Address.AddressLine1.Contains(text));
                    var addressAddressLine2 = new DirectSpecification<Delegate>(c => c.Customer.Address.AddressLine2.Contains(text));
                    var addressStreet = new DirectSpecification<Delegate>(c => c.Customer.Address.Street.Contains(text));
                    var addressLandLineSpec = new DirectSpecification<Delegate>(c => c.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<Delegate>(c => c.Customer.Address.MobileLine.Contains(text));

                    specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | referenceSpec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
                }
            }

            return specification;
        }
    }
}
