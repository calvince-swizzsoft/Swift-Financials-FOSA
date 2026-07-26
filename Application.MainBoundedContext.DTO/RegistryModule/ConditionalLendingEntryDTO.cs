using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class ConditionalLendingEntryDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Conditional Lending")]
        public Guid ConditionalLendingId { get; set; }

        [Display(Name = "Conditional Lending")]
        public string ConditionalLendingDescription { get; set; }

        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }

        [Display(Name = "Customer Type")]
        public byte CustomerType { get; set; }

        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerType)CustomerType);
            }
        }

        [Display(Name = "Salutation")]
        public byte CustomerIndividualSalutation { get; set; }

        [Display(Name = "Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation);
            }
        }

        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [Display(Name = "Nationality")]
        public byte CustomerIndividualNationality { get; set; }

        [Display(Name = "Nationality")]
        public string CustomerIndividualNationalityDescription
        {
            get
            {
                return EnumHelper.GetDescription((Nationality)CustomerIndividualNationality);
            }
        }

        [Display(Name = "Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string PaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [Display(Name = "Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [Display(Name = "First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [Display(Name = "Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [Display(Name = "Name")]
        public string CustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName);
            }
        }

        [Display(Name = "Gender")]
        public byte CustomerIndividualGender { get; set; }

        [Display(Name = "Gender")]
        public string CustomerIndividualGenderDescription
        {
            get
            {
                return EnumHelper.GetDescription((Gender)CustomerIndividualGender);
            }
        }

        [Display(Name = "Marital Status")]
        public byte CustomerIndividualMaritalStatus { get; set; }

        [Display(Name = "Marital Status")]
        public string CustomerIndividualMaritalStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((MaritalStatus)CustomerIndividualMaritalStatus);
            }
        }

        [Display(Name = "Address Line 1")]
        public string CustomerAddressAddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string CustomerAddressAddressLine2 { get; set; }

        [Display(Name = "Street")]
        public string CustomerAddressStreet { get; set; }

        [Display(Name = "Postal Code")]
        public string CustomerAddressPostalCode { get; set; }

        [Display(Name = "City")]
        public string CustomerAddressCity { get; set; }

        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [Display(Name = "Land Line")]
        public string CustomerAddressLandLine { get; set; }

        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [Display(Name = "Station")]
        public Guid? CustomerStationId { get; set; }

        [Display(Name = "Station")]
        public string CustomerStationDescription { get; set; }

        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
