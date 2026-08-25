using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class SupplierBindingModel : BindingModelBase<SupplierBindingModel>
    {
        public SupplierBindingModel()
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
        [Required]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
