using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationRegisterAgg
{
    public static class EducationRegisterSpecifications
    {
        public static Specification<EducationRegister> DefaultSpec()
        {
            Specification<EducationRegister> specification = new TrueSpecification<EducationRegister>();

            return specification;
        }

        public static Specification<EducationRegister> EducationRegisterFullText(string text)
        {
            Specification<EducationRegister> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<EducationRegister>(c => c.Description.Contains(text));

                var postingPeriodSpec = new DirectSpecification<EducationRegister>(c => c.PostingPeriod.Description.Contains(text));

                var educationVenueSpec = new DirectSpecification<EducationRegister>(c => c.EducationVenue.Description.Contains(text));

                specification &= (descriptionSpec | postingPeriodSpec | educationVenueSpec);
            }

            return specification;
        }
    }
}
