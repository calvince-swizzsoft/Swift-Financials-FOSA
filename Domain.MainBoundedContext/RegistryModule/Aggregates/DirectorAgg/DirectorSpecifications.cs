using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DirectorAgg
{
    public static class DirectorSpecifications
    {
        public static Specification<Director> DefaultSpec()
        {
            Specification<Director> specification = new TrueSpecification<Director>();

            return specification;
        }

        public static ISpecification<Director> DirectorWithCustomerId(Guid customerId)
        {
            Specification<Director> specification = new DirectSpecification<Director>(x => x.CustomerId == customerId);

            return specification;
        }

        public static Specification<Director> DirectorFullText(string text)
        {
            Specification<Director> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var serialNumberSpec = new DirectSpecification<Director>(c => c.Customer.SerialNumber == number);

                    var payrollNumbersSpec = new DirectSpecification<Director>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<Director>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));

                    var addressLandLineSpec = new DirectSpecification<Director>(c => c.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<Director>(c => c.Customer.Address.MobileLine.Contains(text));

                    specification &= (serialNumberSpec | payrollNumbersSpec | identificationNumberSpec | addressLandLineSpec | addressMobileLineSpec);
                }
                else
                {
                    var firstNameSpec = new DirectSpecification<Director>(c => c.Customer.Individual.FirstName.Contains(text));
                    var lastNameSpec = new DirectSpecification<Director>(c => c.Customer.Individual.LastName.Contains(text));
                    var payrollNumbersSpec = new DirectSpecification<Director>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<Director>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                    var reference1Spec = new DirectSpecification<Director>(c => c.Customer.Reference1.Contains(text));

                    var addressEmail = new DirectSpecification<Director>(c => c.Customer.Address.Email.Contains(text));
                    var addressCity = new DirectSpecification<Director>(c => c.Customer.Address.City.Contains(text));
                    var addressPostalCode = new DirectSpecification<Director>(c => c.Customer.Address.PostalCode.Contains(text));
                    var addressAddressLine1 = new DirectSpecification<Director>(c => c.Customer.Address.AddressLine1.Contains(text));
                    var addressAddressLine2 = new DirectSpecification<Director>(c => c.Customer.Address.AddressLine2.Contains(text));
                    var addressStreet = new DirectSpecification<Director>(c => c.Customer.Address.Street.Contains(text));
                    var addressLandLineSpec = new DirectSpecification<Director>(c => c.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<Director>(c => c.Customer.Address.MobileLine.Contains(text));

                    specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
                }
            }

            return specification;
        }
    }
}
