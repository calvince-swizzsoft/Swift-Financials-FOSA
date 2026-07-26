using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationVenueAgg
{
    public static class EducationVenueSpecifications
    {
        public static Specification<EducationVenue> DefaultSpec()
        {
            Specification<EducationVenue> specification = new TrueSpecification<EducationVenue>();

            return specification;
        }

        public static Specification<EducationVenue> EducationVenueFullText(string text)
        {
            Specification<EducationVenue> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<EducationVenue>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
