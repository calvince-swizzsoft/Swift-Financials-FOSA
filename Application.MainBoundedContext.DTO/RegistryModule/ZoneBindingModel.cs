using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public  class ZoneBindingModel : BindingModelBase<ZoneBindingModel>
    {
        public ZoneBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        [ValidGuid]
        public Guid DivisionId { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public string DivisionDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public Guid DivisionEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string DivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public IList<StationDTO> Stations { get; set; }
    }
}
