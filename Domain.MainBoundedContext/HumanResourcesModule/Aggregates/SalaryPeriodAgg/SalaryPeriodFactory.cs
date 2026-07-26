using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryPeriodAgg
{
    public static class SalaryPeriodFactory
    {
        public static SalaryPeriod CreateSalaryPeriod(Guid postingPeriodId, int month, int employeeCategory, decimal taxReliefAmount, decimal maximumProvidentFundReliefAmount, decimal maximumInsuranceReliefAmount, string remarks)
        {
            var salaryPeriod = new SalaryPeriod();

            salaryPeriod.GenerateNewIdentity();

            salaryPeriod.PostingPeriodId = postingPeriodId;

            salaryPeriod.Month = (byte)month;

            salaryPeriod.EmployeeCategory = (byte)employeeCategory;

            salaryPeriod.TaxReliefAmount = taxReliefAmount;

            salaryPeriod.MaximumProvidentFundReliefAmount = maximumProvidentFundReliefAmount;

            salaryPeriod.MaximumInsuranceReliefAmount = maximumInsuranceReliefAmount;

            salaryPeriod.Remarks = remarks;

            salaryPeriod.CreatedDate = DateTime.Now;

            return salaryPeriod;
        }
    }
}
