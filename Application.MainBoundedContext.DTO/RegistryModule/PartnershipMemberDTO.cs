
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class PartnershipMemberDTO : BindingModelBase<PartnershipMemberDTO>
    {
        public PartnershipMemberDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Partnership")]
        [ValidGuid]
        public Guid PartnershipId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int Salutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string SalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), Salutation) ? EnumHelper.GetDescription((Salutation)Salutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        [Required]
        public string LastName { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1} {2}", SalutationDescription, FirstName, LastName);
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public int IdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string IdentityCardTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(IdentityCardType), IdentityCardType) ? EnumHelper.GetDescription((IdentityCardType)IdentityCardType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        [Required]
        public string IdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string PayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public int Gender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string GenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), Gender) ? EnumHelper.GetDescription((Gender)Gender) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Relationship")]
        public int Relationship { get; set; }

        [DataMember]
        [Display(Name = "Relationship")]
        public string RelationshipDescription
        {
            get
            {
                return Enum.IsDefined(typeof(PartnershipRelationship), Relationship) ? EnumHelper.GetDescription((PartnershipRelationship)Relationship) : string.Empty;
            }
        }

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
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid email address!")]
        public string AddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string AddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        [RegularExpression(@"^\+(?:[0-9]??){6,14}[0-9]$", ErrorMessage = "The mobile number should start with a plus sign, followed by the country code and national number")]
        public string AddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Signatory?")]
        public bool Signatory { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
