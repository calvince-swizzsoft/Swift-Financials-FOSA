using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryPeriodAgg
{
    public static class SalaryPeriodSpecifications
    {
        public static Specification<SalaryPeriod> DefaultSpec()
        {
            Specification<SalaryPeriod> specification = new TrueSpecification<SalaryPeriod>();

            return specification;
        }

        public static ISpecification<SalaryPeriod> SalaryPeriodWithPostingPeriodIdMonthAndEmployeeType(Guid postingPeriodId, int month, int employeeCategory)
        {
            Specification<SalaryPeriod> specification = new DirectSpecification<SalaryPeriod>(x => x.PostingPeriodId == postingPeriodId && x.Month == month && x.EmployeeCategory == employeeCategory);

            return specification;
        }

        public static Specification<SalaryPeriod> SalaryPeriodFullText(string text)
        {
            Specification<SalaryPeriod> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<SalaryPeriod>(c => c.Remarks.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static Specification<SalaryPeriod> SalaryPeriodFullText(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<SalaryPeriod> specification = new DirectSpecification<SalaryPeriod>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<SalaryPeriod>(c => c.Remarks.Contains(text));

                var createdBySpec = new DirectSpecification<SalaryPeriod>(c => c.CreatedBy.Contains(text));

                specification &= (remarksSpec | createdBySpec);
            }

            return specification;
        }
    }
}
