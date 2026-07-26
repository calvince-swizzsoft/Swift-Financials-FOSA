using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class SystemPermissionTypeInBranchDTO : BindingModelBase<SystemPermissionTypeInBranchDTO>
    {
        public SystemPermissionTypeInBranchDTO()
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
        [Display(Name = "System Permission Type")]
        public string SystemPermissionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemPermissionType), SystemPermissionType) ? EnumHelper.GetDescription((SystemPermissionType)SystemPermissionType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int BranchCode { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
