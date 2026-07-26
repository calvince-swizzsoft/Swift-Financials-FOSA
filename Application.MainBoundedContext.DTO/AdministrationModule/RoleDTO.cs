using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class RoleDTO
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
