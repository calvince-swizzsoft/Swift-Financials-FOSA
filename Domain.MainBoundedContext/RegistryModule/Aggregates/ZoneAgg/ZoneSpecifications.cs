using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg
{
    public static class ZoneSpecifications
    {
        public static Specification<Zone> DefaultSpec()
        {
            Specification<Zone> specification = new TrueSpecification<Zone>();

            return specification;
        }

        public static ISpecification<Zone> ZoneWithDivisionId(Guid divisionId)
        {
            Specification<Zone> specification = DefaultSpec();

            if (divisionId != null && divisionId != Guid.Empty)
            {
                specification &= new DirectSpecification<Zone>(x => x.DivisionId == divisionId);
            }

            return specification;
        }

        public static Specification<Zone> ZoneFullText(string text)
        {
            Specification<Zone> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Zone>(c => c.Description.Contains(text));

                specification &= descriptionSpec;
            }

            return specification;
        }

        public static ISpecification<Zone> ZoneWithEmployerId(Guid employerId)
        {
            Specification<Zone> specification = DefaultSpec();

            if (employerId != null && employerId != Guid.Empty)
            {
                specification &= new DirectSpecification<Zone>(x => x.Division.EmployerId == employerId);
            }

            return specification;
        }
    }
}
