using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MicroCreditModule
{
    public class MicroCreditGroupMemberDTO : BindingModelBase<MicroCreditGroupMemberDTO>
    {
        public MicroCreditGroupMemberDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Micro Credit Group")]
        [ValidGuid]
        public Guid MicroCreditGroupId { get; set; }

        [DataMember]
        [Display(Name = "Micro Credit Group Customer")]
        public Guid MicroCreditGroupCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public int CustomerIndividualIdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string CustomerIndividualIdentityCardTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(IdentityCardType), CustomerIndividualIdentityCardType) ? EnumHelper.GetDescription((IdentityCardType)CustomerIndividualIdentityCardType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [Display(Name = "Identity Card Serial #")]
        public string CustomerIndividualIdentityCardSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public int CustomerIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string CustomerIndividualNationalityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Nationality), CustomerIndividualNationality) ? EnumHelper.GetDescription((Nationality)CustomerIndividualNationality) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string CustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public int CustomerIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string CustomerIndividualGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), CustomerIndividualGender) ? EnumHelper.GetDescription((Gender)CustomerIndividualGender) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public int CustomerIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string CustomerIndividualMaritalStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MaritalStatus), CustomerIndividualMaritalStatus) ? EnumHelper.GetDescription((MaritalStatus)CustomerIndividualMaritalStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string CustomerAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string CustomerAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string CustomerAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string CustomerAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string CustomerAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string CustomerAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public Guid? CustomerStationId { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string CustomerStationDescription { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public int Designation { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public string DesignationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MicroCreditGroupMemberDesignation), Designation) ? EnumHelper.GetDescription((MicroCreditGroupMemberDesignation)Designation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Loan Cycle")]
        public int LoanCycle { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        //Added Properties

        
        [DataMember]
        [Display(Name = "Employer")]
        public string Employer { get; set; }
        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerAccountCustomerReference1 { get; set; }
        [DataMember]
        [Display(Name = "MemberShip Number")]
        public string CustomerAccountCustomerReference2 { get; set; }
        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerAccountCustomerReference3 { get; set; }
        

        [DataMember]
        [Display(Name = "Account Status")]
        public int AccountStatus { get; set; }


        [DataMember]
        [Display(Name = "Principal")]
        public decimal Principal { get; set; }

        [DataMember]
        [Display(Name = "Interest")]
        public decimal Interest { get; set; }

        [DataMember]
        [Display(Name = "Primary Description")]
        [Required]
        public string PrimaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Secondary Description")]
        [Required]
        public string SecondaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription { get; set; }



        [DataMember]
        [Display(Name = "Status")]
        public string AccountStatusDescription { get; set; }


        [DataMember]
        [Display(Name = "Available Balance")]
        public decimal AvailableBalance { get; set; }

        [DataMember]
        [Display(Name = "Book Balance")]
        public decimal BookBalance { get; set; }


        [DataMember]
        [Display(Name = "Account Number")]
        public String AccountNumber { get; set; }

        

       

        

        [DataMember]
        [Display(Name = "Personal Id Number")]
        public string CustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullAccountNumber { get; set; }


        [DataMember]
        public string errorMsg { get; set; }
    }
}
