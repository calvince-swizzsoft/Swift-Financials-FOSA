using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg
{
    public static class EmployeeAppraisalPeriodSpecifications
    {
        public static Specification<EmployeeAppraisalPeriod> DefaultSpec()
        {
            Specification<EmployeeAppraisalPeriod> specification = new TrueSpecification<EmployeeAppraisalPeriod>();

            return specification;
        }

        public static Specification<EmployeeAppraisalPeriod> CurrentEmployeeAppraisalPeriod()
        {
            return new DirectSpecification<EmployeeAppraisalPeriod>(x =>!x.IsLocked && x.IsActive);
        }

        public static Specification<EmployeeAppraisalPeriod> EmployeeAppraisalPeriodFullText(string text)
        {
            Specification<EmployeeAppraisalPeriod> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<EmployeeAppraisalPeriod>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
