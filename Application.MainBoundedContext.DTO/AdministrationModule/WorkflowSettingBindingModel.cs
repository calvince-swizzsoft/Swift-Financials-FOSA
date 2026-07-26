using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowSettingBindingModel : BindingModelBase<WorkflowSettingBindingModel>
    {
        public WorkflowSettingBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "System Permission Type")]
        public int SystemPermissionType { get; set; }

        [DataMember]
        [Display(Name = "Require Biometrics?")]
        public bool RequireBiometrics { get; set; }
    }
}
