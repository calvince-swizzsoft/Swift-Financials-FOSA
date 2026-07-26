
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
using System.Collections.ObjectModel;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CustomerAccountSignatoryDTO : BindingModelBase<CustomerAccountSignatoryDTO>
    {
        public CustomerAccountSignatoryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

        public CustomerAccountDTO Customers { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

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
                return Enum.IsDefined(typeof(SignatoryRelationship), Relationship) ? EnumHelper.GetDescription((SignatoryRelationship)Relationship) : string.Empty;
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
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public ObservableCollection<CustomerAccountSignatoryDTO> customerAccountSignatoryDTOs { get; set; }
    }
}
