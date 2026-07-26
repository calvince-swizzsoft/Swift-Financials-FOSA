using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowSettingDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "System Permission Type")]
        public int SystemPermissionType { get; set; }

        [Display(Name = "Require Biometrics?")]
        public bool RequireBiometrics { get; set; }
    }
}
