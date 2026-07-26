using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg
{
    public static class CustomerDocumentSpecifications
    {
        public static Specification<CustomerDocument> DefaultSpec()
        {
            Specification<CustomerDocument> specification = new TrueSpecification<CustomerDocument>();

            return specification;
        }

        public static ISpecification<CustomerDocument> CustomerDocumentWithCustomerId(Guid customerId)
        {
            Specification<CustomerDocument> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerDocument>(x => x.CustomerId == customerId);
            }

            return specification;
        }

        public static ISpecification<CustomerDocument> CustomerDocumentWithCustomerIdAndType(Guid customerId, int type)
        {
            Specification<CustomerDocument> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerDocument>(x => x.CustomerId == customerId && x.Type == type);
            }

            return specification;
        }

        public static Specification<CustomerDocument> CustomerDocumentFullText(string text)
        {
            Specification<CustomerDocument> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<CustomerDocument>(c => c.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<CustomerDocument>(c => c.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<CustomerDocument>(c => c.Customer.Reference3.Contains(text));

                var addressEmailSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.Email.Contains(text));
                var addressCitySpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.City.Contains(text));
                var addressPostalCodeSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1Spec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2Spec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.AddressLine2.Contains(text));
                var addressStreetSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<CustomerDocument>(c => c.Customer.Address.MobileLine.Contains(text));

                var fileTitleSpec = new DirectSpecification<CustomerDocument>(c => c.FileTitle.Contains(text));
                var fileDescriptionSpec = new DirectSpecification<CustomerDocument>(c => c.FileDescription.Contains(text));
                var fileMIMETypeSpec = new DirectSpecification<CustomerDocument>(c => c.FileMIMEType.Contains(text));

                specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmailSpec | addressCitySpec | addressPostalCodeSpec | addressAddressLine1Spec | addressAddressLine2Spec | addressStreetSpec | addressLandLineSpec | addressMobileLineSpec
                | fileTitleSpec | fileDescriptionSpec | fileMIMETypeSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var serialNumberSpec = new DirectSpecification<CustomerDocument>(x => x.Customer.SerialNumber == number);

                    specification |= serialNumberSpec;
                }
            }

            return specification;
        }
    }
}
