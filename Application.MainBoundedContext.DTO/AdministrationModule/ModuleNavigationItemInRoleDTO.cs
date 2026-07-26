using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class ModuleNavigationItemInRoleDTO : BindingModelBase<ModuleNavigationItemInRoleDTO>
    {
        public ModuleNavigationItemInRoleDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Item")]
        public Guid ModuleNavigationItemId { get; set; }

        [DataMember]
        [Display(Name = "Module")]
        public Guid ModuleNavigationItemModuleId { get; set; }

        [DataMember]
        [Display(Name = "Module")]
        public string ModuleNavigationItemModuleDescription { get; set; }

        [DataMember]
        [Display(Name = "Item Code")]
        public int ModuleNavigationItemItemCode { get; set; }

        [DataMember]
        [Display(Name = "Item")]
        public string ModuleNavigationItemItemDescription { get; set; }

        [DataMember]
        [Display(Name = "Parent Item Code")]
        public int ModuleNavigationItemParentItemCode { get; set; }

        [DataMember]
        [Display(Name = "Parent Item")]
        public string ModuleNavigationItemParentItemDescription { get; set; }

        [DataMember]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
