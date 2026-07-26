using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class SupplierDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [DataMember]
        [Display(Name = "Land-Line")]
        public string LandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile-Line")]
        public string MobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }
        public string ChartofAccountName { get; set; }

    }
}
