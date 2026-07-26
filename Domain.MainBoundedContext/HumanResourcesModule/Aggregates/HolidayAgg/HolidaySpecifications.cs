using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg
{
    public static class HolidaySpecifications
    {
        public static Specification<Holiday> DefaultSpec()
        {
            Specification<Holiday> specification = new TrueSpecification<Holiday>();

            return specification;
        }

        public static Specification<Holiday> HolidayWithPostingPeriodId(Guid postingPeriodId)
        {
            Specification<Holiday> specification = new DirectSpecification<Holiday>(c => c.PostingPeriodId == postingPeriodId);

            return specification;
        }

        public static Specification<Holiday> HolidayFullText(string text)
        {
            Specification<Holiday> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Holiday>(c => c.Description.Contains(text));

                specification &= descriptionSpec;
            }

            return specification;
        }

        public static Specification<Holiday> HolidayWithinDurationDates(DateTime durationStartDate, DateTime durationEndDate)
        {
            Specification<Holiday> specification = new DirectSpecification<Holiday>(c => c.Duration.StartDate >= durationStartDate && c.Duration.EndDate <= durationEndDate);

            return specification;
        }
    }
}
