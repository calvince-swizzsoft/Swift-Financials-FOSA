using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodRecommendationAgg
{
    public static class EmployeeAppraisalPeriodRecommendationFactory
    {
        public static EmployeeAppraisalPeriodRecommendation CreateEmployeeAppraisalPeriodRecommendation(Guid employeeId, Guid employeeAppraisalPeriodId, string recommendation)
        {
            var employeeAppraisalPeriodRecommendation = new EmployeeAppraisalPeriodRecommendation();

            employeeAppraisalPeriodRecommendation.GenerateNewIdentity();

            employeeAppraisalPeriodRecommendation.EmployeeId = employeeId;

            employeeAppraisalPeriodRecommendation.EmployeeAppraisalPeriodId = employeeAppraisalPeriodId;

            employeeAppraisalPeriodRecommendation.Recommendation = recommendation;

            employeeAppraisalPeriodRecommendation.CreatedDate = DateTime.Now;

            return employeeAppraisalPeriodRecommendation;
        }
    }
}
