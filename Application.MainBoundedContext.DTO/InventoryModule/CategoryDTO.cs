using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class CategoryDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}