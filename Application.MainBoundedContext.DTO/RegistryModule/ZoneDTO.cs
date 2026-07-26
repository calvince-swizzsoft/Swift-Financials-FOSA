using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class ZoneDTO
    { 
        [Display(Name = "Id")]
        public Guid Id { get; set; }
        
        [Display(Name = "Division")]
        public Guid DivisionId { get; set; }
        
        [Display(Name = "Division")]
        public string DivisionDescription { get; set; }

        [Display(Name = "Employer")]
        public Guid DivisionEmployerId { get; set; }
        
        [Display(Name = "Employer")]
        public string DivisionEmployerDescription { get; set; }
        
        [Display(Name = "Name")]
        public string Description { get; set; }
        
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public IList<StationDTO> Stations { get; set; }
    }
}
