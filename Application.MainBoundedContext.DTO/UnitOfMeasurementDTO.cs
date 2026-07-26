using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
namespace Application.MainBoundedContext.DTO
{
    public class UnitOfMeasurementDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Abbreviation")]
        public string Abbreviation { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
    }
}
