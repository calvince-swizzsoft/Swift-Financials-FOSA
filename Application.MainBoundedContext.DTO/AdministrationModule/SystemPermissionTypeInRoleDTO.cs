using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class SystemPermissionTypeInRoleDTO : BindingModelBase<SystemPermissionTypeInRoleDTO>
    {
        public SystemPermissionTypeInRoleDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Operation")]
        public int SystemPermissionType { get; set; }

        [DataMember]
        [Display(Name = "Operation")]
        public string SystemPermissionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemPermissionType), SystemPermissionType) ? EnumHelper.GetDescription((SystemPermissionType)SystemPermissionType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        [DataMember]
        [Display(Name = "Branch Description")]
        public string BranchDescription { get; set; }
        [DataMember]
        [Display(Name = "Role")]
        public Guid? RoleId { get; set; }

        [DataMember]
        [Display(Name = "Required Approvers")]
        public int RequiredApprovers { get; set; }

        [DataMember]
        [Display(Name = "BranchId")]
        public Guid BranchId { get; set; }
        [DataMember]
        [Display(Name = "Approval Priority")]
        public int ApprovalPriority { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public ObservableCollection<SystemPermissionTypeInRoleDTO> systemPermissionTypeInRoles { get; set; }

        public ObservableCollection<BranchDTO> systemPermissionTypeInBranchDTOs { get; set; }

        // List of roles that can be associated with the selected SystemPermissionType
        public ObservableCollection<string> Roles;

        // List of branches that can be associated with the selected SystemPermissionType
        public ObservableCollection<string> Branches;


    }

}
