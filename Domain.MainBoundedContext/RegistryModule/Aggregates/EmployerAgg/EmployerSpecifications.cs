using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EmployerAgg
{
    public static class EmployerSpecifications
    {
        public static Specification<Employer> DefaultSpec()
        {
            Specification<Employer> specification = new TrueSpecification<Employer>();

            return specification;
        }

        public static Specification<Employer> EmployerFullText(string text)
        {
            Specification<Employer> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Employer>(c => c.Description.Contains(text));
                var addressEmail = new DirectSpecification<Employer>(c => c.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<Employer>(c => c.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<Employer>(c => c.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<Employer>(c => c.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<Employer>(c => c.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<Employer>(c => c.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<Employer>(c => c.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<Employer>(c => c.Address.MobileLine.Contains(text));

                specification &= (descriptionSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }
    }
}
