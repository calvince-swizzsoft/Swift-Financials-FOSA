using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class NextOfKinBindingModel : BindingModelBase<NextOfKinBindingModel>
    {
        public NextOfKinBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public byte CustomerSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)CustomerSalutation);
            }
        }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", CustomerSalutationDescription, CustomerFirstName, CustomerLastName);
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public byte Salutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string SalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)Salutation);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public byte Gender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string GenderDescription
        {
            get
            {
                return EnumHelper.GetDescription((Gender)Gender);
            }
        }

        [DataMember]
        [Display(Name = "Relationship")]
        public byte Relationship { get; set; }

        [DataMember]
        [Display(Name = "Relationship")]
        public string RelationshipDescription
        {
            get
            {
                return EnumHelper.GetDescription((NextOfKinRelationship)Relationship);
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
        public byte IdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string IdentityCardTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IdentityCardType)IdentityCardType);
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string IdentityCardNumber { get; set; }

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
        [Display(Name = "Nominated Percentage")]
        public double NominatedPercentage { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
