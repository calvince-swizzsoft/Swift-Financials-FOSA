using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg
{
    public static class EmployeeAppraisalPeriodFactory
    {
        public static EmployeeAppraisalPeriod CreateEmployeeAppraisalPeriod(string description, Duration duration)
        {
            var employeeAppraisalPeriod = new EmployeeAppraisalPeriod();

            employeeAppraisalPeriod.GenerateNewIdentity();

            employeeAppraisalPeriod.Description = description;

            employeeAppraisalPeriod.Duration = duration;

            employeeAppraisalPeriod.CreatedDate = DateTime.Now;

            employeeAppraisalPeriod.UnLock();

            return employeeAppraisalPeriod;
        }
    }
}
