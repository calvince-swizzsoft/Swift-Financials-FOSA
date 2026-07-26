using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodRecommendationAgg
{
    public static class EmployeeAppraisalPeriodRecommendationSpecifications
    {
        public static Specification<EmployeeAppraisalPeriodRecommendation> DefaultSpec()
        {
            Specification<EmployeeAppraisalPeriodRecommendation> specification = new TrueSpecification<EmployeeAppraisalPeriodRecommendation>();

            return specification;
        }

        public static ISpecification<EmployeeAppraisalPeriodRecommendation> EmployeeAppraisalPeriodRecommendation(Guid employeeId, Guid employeeAppraisalPeriodId)
        {
            Specification<EmployeeAppraisalPeriodRecommendation> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty && employeeAppraisalPeriodId != null && employeeAppraisalPeriodId != Guid.Empty)
            {
                specification &= new DirectSpecification<EmployeeAppraisalPeriodRecommendation>(x => x.EmployeeId == employeeId && x.EmployeeAppraisalPeriodId == employeeAppraisalPeriodId);
            }

            return specification;
        }

    }
}
