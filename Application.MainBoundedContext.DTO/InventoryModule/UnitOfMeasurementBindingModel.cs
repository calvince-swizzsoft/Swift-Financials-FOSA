using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class UnitOfMeasurementBindingModel : BindingModelBase<UnitOfMeasurementBindingModel>
    {
        public UnitOfMeasurementBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Contains")]
        public decimal? Contains { get; set; }

        [DataMember]
        [Display(Name = "Of Base Units")]
        public Guid? BaseUnitId { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
