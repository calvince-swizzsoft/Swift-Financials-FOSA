using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class DivisionDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Employer")]
        public Guid EmployerId { get; set; }

        [Display(Name = "Employer")]
        public string EmployerDescription { get; set; }

        [Display(Name = "Name")]
        public string Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
