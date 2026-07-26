using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class EmployerDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Description { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressAddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressAddressLine2 { get; set; }

        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [Display(Name = "Postal Code")]
        public string AddressPostalCode { get; set; }

        [Display(Name = "City")]
        public string AddressCity { get; set; }

        [Display(Name = "E-mail")]
        public string AddressEmail { get; set; }

        [Display(Name = "Land Line")]
        public string AddressLandLine { get; set; }

        [Display(Name = "Mobile Line")]
        public string AddressMobileLine { get; set; }

        [Display(Name = "Retirement Age")]
        public byte RetirementAge { get; set; }

        [Display(Name = "Enforce retirement age?")]
        public bool EnforceRetirementAge { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public IList<DivisionDTO> Divisions { get; set; }

        public IList<StationDTO> Stations { get; set; }


        public IList<ZoneDTO> Zones { get; set; }
    }
}
