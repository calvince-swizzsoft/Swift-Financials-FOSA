using Application.Seedwork;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class RoleBindingModel : BindingModelBase<RoleBindingModel>
    {
        public RoleBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [DataMember]
        [Display(Name = "Role Name")]
        [Required]
        public string Name { get; set; }
    }
}
