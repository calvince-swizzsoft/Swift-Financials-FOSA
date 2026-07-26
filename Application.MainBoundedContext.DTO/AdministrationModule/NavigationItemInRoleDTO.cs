using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class NavigationItemInRoleDTO : BindingModelBase<NavigationItemInRoleDTO>
    {
        public NavigationItemInRoleDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Item")]
        [ValidGuid]
        public Guid NavigationItemId { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        public string NavigationItemDescription { get; set; }

        [DataMember]
        [Display(Name = "Code")]
        public int navigationItemCode { get; set; }

        [DataMember]
        [Display(Name = "Icon")]
        public string NavigationItemIcon { get; set; }

        [DataMember]
        [Display(Name = "Controller")]
        public string NavigationItemControllerName { get; set; }

        [DataMember]
        [Display(Name = "Action")]
        public string NavigationItemActionName { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public Guid? NavigationItemParentId { get; set; }

        [DataMember]
        [Display(Name = "Area Name")]
        public string NavigationItemAreaName { get; set; }

        [DataMember]
        [Display(Name = "Parent Code")]
        public int NavigationItemAreaCode { get; set; }

        [DataMember]
        [Display(Name = "Is Area?")]
        public bool NavigationItemIsArea { get; set; }

        [DataMember]
        [Display(Name = "Role Name")]
        [Required]
        public string RoleName { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public SystemPermissionTypeInRoleDTO h;
    }
}