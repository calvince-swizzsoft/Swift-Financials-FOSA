using Domain.Seedwork;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowSetting
{
    public class WorkflowSetting : Entity
    {
        public int SystemPermissionType { get; set; }

        public bool RequireBiometrics { get; set; }
    }
}
