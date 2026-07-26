using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO
{
    public class EnumerationDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Key")]
        public string Key { get; set; }

        [Display(Name = "Value")]
        public int Value { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
