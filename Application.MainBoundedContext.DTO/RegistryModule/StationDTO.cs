using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    [DataContract]
    public class StationDTO : BindingModelBase<StationDTO>
    {
        public StationDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Zone")]
        public Guid ZoneId { get; set; }

        [DataMember]
        [Display(Name = "Zone")]
        public string ZoneDescription { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public Guid ZoneDivisionId { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public string ZoneDivisionDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public Guid ZoneDivisionEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string ZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string AddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string AddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string AddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string AddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string AddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string AddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string AddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}