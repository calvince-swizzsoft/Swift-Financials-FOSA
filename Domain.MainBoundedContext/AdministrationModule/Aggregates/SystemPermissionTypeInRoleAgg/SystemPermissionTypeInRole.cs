using Domain.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInRoleAgg
{
    public class SystemPermissionTypeInRole : Entity
    {
        [Index("IX_SystemPermissionTypeInRole_SystemPermissionType")]
        public int SystemPermissionType { get; set; }

        [Index("IX_SystemPermissionTypeInRole_RoleName")]
        public string RoleName { get; set; }

        public int RequiredApprovers { get; set; }

        public int ApprovalPriority { get; set; }
    }
}
