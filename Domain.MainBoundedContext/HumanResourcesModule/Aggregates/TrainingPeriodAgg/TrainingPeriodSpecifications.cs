using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodAgg
{
    public class TrainingPeriodSpecifications
    {
        public static Specification<TrainingPeriod> DefaultSpec()
        {
            Specification<TrainingPeriod> specification = new TrueSpecification<TrainingPeriod>();

            return specification;
        }

        public static Specification<TrainingPeriod> TrainingPeriodWithDurationAndFilterText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<TrainingPeriod> specification = new DirectSpecification<TrainingPeriod>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<TrainingPeriod>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Description.Contains(text));

                var createdBySpec = new DirectSpecification<TrainingPeriod>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.CreatedBy.Contains(text));

                specification &= (descriptionSpec | createdBySpec);
            }

            return specification;
        }
    }
}
