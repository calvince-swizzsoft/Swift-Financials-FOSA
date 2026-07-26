using Domain.Seedwork.Specification;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowSetting
{
    public static class WorkflowSettingSpecifications
    {
        public static Specification<WorkflowSetting> DefaultSpec()
        {
            Specification<WorkflowSetting> specification = new TrueSpecification<WorkflowSetting>();

            return specification;
        }

        public static Specification<WorkflowSetting> WorkflowSettingBySystemPermissionType(int systemPermissionType)
        {
            Specification<WorkflowSetting> specification = new DirectSpecification<WorkflowSetting>(x => x.SystemPermissionType == systemPermissionType);

            return specification;
        }
    }
}
