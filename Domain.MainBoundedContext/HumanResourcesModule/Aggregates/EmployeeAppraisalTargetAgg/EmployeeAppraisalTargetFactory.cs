using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalTargetAgg
{
    public static class EmployeeAppraisalTargetFactory
    {
        public static EmployeeAppraisalTarget CreateEmployeeAppraisalTarget(Guid? parentId, int type, int answerType)
        {
            var employeeAppraisalTarget = new EmployeeAppraisalTarget();

            employeeAppraisalTarget.GenerateNewIdentity();

            employeeAppraisalTarget.ParentId = parentId;

            employeeAppraisalTarget.Type = (byte)type;

            employeeAppraisalTarget.AnswerType = (byte)answerType;

            employeeAppraisalTarget.CreatedDate = DateTime.Now;

            return employeeAppraisalTarget;
        }
    }
}
