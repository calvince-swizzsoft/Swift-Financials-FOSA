using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg
{
    public static class StationSpecifications
    {
        public static Specification<Station> DefaultSpec()
        {
            Specification<Station> specification = new TrueSpecification<Station>();

            return specification;
        }

        public static Specification<Station> StationFullText(string text)
        {
            Specification<Station> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Station>(c => c.Description.Contains(text));
                var addressEmail = new DirectSpecification<Station>(c => c.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<Station>(c => c.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<Station>(c => c.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<Station>(c => c.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<Station>(c => c.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<Station>(c => c.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<Station>(c => c.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<Station>(c => c.Address.MobileLine.Contains(text));

                var zoneSpec = new DirectSpecification<Station>(c => c.Zone.Description.Contains(text));
                var divisionSpec = new DirectSpecification<Station>(c => c.Zone.Division.Description.Contains(text));
                var employerSpec = new DirectSpecification<Station>(c => c.Zone.Division.Employer.Description.Contains(text));

                specification &= (descriptionSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                    | zoneSpec | divisionSpec | employerSpec);
            }

            return specification;
        }

        public static ISpecification<Station> StationWithEmployerId(Guid employerId)
        {
            Specification<Station> specification = DefaultSpec();

            if (employerId != null && employerId != Guid.Empty)
            {
                specification &= new DirectSpecification<Station>(x => x.Zone.Division.EmployerId == employerId);
            }

            return specification;
        }

        public static ISpecification<Station> StationWithZoneId(Guid zoneId)
        {
            Specification<Station> specification = DefaultSpec();

            if (zoneId != null && zoneId != Guid.Empty)
            {
                specification &= new DirectSpecification<Station>(x => x.ZoneId == zoneId);
            }

            return specification;
        }

        public static ISpecification<Station> StationWithDivisionId(Guid divisionId)
        {
            Specification<Station> specification = DefaultSpec();

            if (divisionId != null && divisionId != Guid.Empty)
            {
                specification &= new DirectSpecification<Station>(x => x.Zone.DivisionId == divisionId);
            }

            return specification;
        }
    }
}
