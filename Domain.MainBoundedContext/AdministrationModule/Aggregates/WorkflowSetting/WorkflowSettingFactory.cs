using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowSetting
{
    public static class WorkflowSettingFactory
    {
        public static WorkflowSetting CreateWorkflowSetting(int systemPermissionType, bool requireBiometrics)
        {
            var workflowSetting = new WorkflowSetting();

            workflowSetting.GenerateNewIdentity();

            workflowSetting.SystemPermissionType = systemPermissionType;

            workflowSetting.RequireBiometrics = requireBiometrics;

            workflowSetting.CreatedDate = DateTime.Now;

            return workflowSetting;

        }
    }
}
