using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalAgg
{
    public static class EmployeeAppraisalFactory
    {
        public static EmployeeAppraisal CreateEmployeeAppraisal(Guid employeeAppraisalPeriodId, Guid employeeId, Guid branchId, Guid employeeAppraisalTargetId, string appraiseeAnswer)
        {
            var employeeAppraisal = new EmployeeAppraisal();

            employeeAppraisal.GenerateNewIdentity();

            employeeAppraisal.EmployeeAppraisalPeriodId = employeeAppraisalPeriodId;

            employeeAppraisal.EmployeeId = employeeId;

            employeeAppraisal.BranchId = branchId;

            employeeAppraisal.EmployeeAppraisalTargetId = employeeAppraisalTargetId;

            employeeAppraisal.AppraiseeAnswer = appraiseeAnswer;

            employeeAppraisal.CreatedDate = DateTime.Now;

            return employeeAppraisal;

        }
    }
}
