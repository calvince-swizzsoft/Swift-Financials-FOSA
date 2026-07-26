using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodRecommendationAgg
{
    public class EmployeeAppraisalPeriodRecommendation : Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public Guid EmployeeAppraisalPeriodId { get; set; }

        public virtual EmployeeAppraisalPeriod EmployeeAppraisalPeriod { get; private set; }

        public string Recommendation { get; set; }
    }
}
