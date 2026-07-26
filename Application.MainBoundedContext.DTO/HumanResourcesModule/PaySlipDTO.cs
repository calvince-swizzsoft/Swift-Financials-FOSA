using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class PaySlipDTO : BindingModelBase<PaySlipDTO>
    {
        public PaySlipDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Salary Period")]
        [ValidGuid]
        public Guid SalaryPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public Guid SalaryPeriodPostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string SalaryPeriodPostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Month")]
        public int SalaryPeriodMonth { get; set; }

        [DataMember]
        [Display(Name = "Month")]
        public string SalaryPeriodMonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), SalaryPeriodMonth) ? EnumHelper.GetDescription((Month)SalaryPeriodMonth) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salary Period Status")]
        public int SalaryPeriodStatus { get; set; }

        [DataMember]
        [Display(Name = "Salary Period Status")]
        public string SalaryPeriodStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryPeriodStatus), SalaryPeriodStatus) ? EnumHelper.GetDescription((SalaryPeriodStatus)SalaryPeriodStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salary Card")]
        [ValidGuid]
        public Guid SalaryCardId { get; set; }

        [DataMember]
        [Display(Name = "Salary Group")]
        [ValidGuid]
        public Guid SalaryCardSalaryGroupId { get; set; }

        [DataMember]
        [Display(Name = "Salary Group")]
        public string SalaryCardSalaryGroupDescription { get; set; }

        [DataMember]
        [Display(Name = "Card Number")]
        public int SalaryCardCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Card Number")]
        public string PaddedSalaryCardCardNumber
        {
            get
            {
                return string.Format("{0}", SalaryCardCardNumber).PadLeft(6, '0');
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid SalaryCardEmployeeCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int SalaryCardEmployeeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string SalaryCardEmployeeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), SalaryCardEmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)SalaryCardEmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public int SalaryCardEmployeeCustomerIndividualIdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string SalaryCardEmployeeCustomerIndividualIdentityCardTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(IdentityCardType), SalaryCardEmployeeCustomerIndividualIdentityCardType) ? EnumHelper.GetDescription((IdentityCardType)SalaryCardEmployeeCustomerIndividualIdentityCardType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string SalaryCardEmployeeCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public int SalaryCardEmployeeCustomerIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string SalaryCardEmployeeCustomerIndividualNationalityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Nationality), SalaryCardEmployeeCustomerIndividualNationality) ? EnumHelper.GetDescription((Nationality)SalaryCardEmployeeCustomerIndividualNationality) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int SalaryCardEmployeeCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedSalaryCardEmployeeCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", SalaryCardEmployeeCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string SalaryCardEmployeeCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string SalaryCardEmployeeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string SalaryCardEmployeeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        public string SalaryCardEmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", SalaryCardEmployeeCustomerIndividualSalutationDescription, SalaryCardEmployeeCustomerIndividualFirstName, SalaryCardEmployeeCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public int SalaryCardEmployeeCustomerIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string SalaryCardEmployeeCustomerIndividualGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), SalaryCardEmployeeCustomerIndividualGender) ? EnumHelper.GetDescription((Gender)SalaryCardEmployeeCustomerIndividualGender) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public int SalaryCardEmployeeCustomerIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string SalaryCardEmployeeCustomerIndividualMaritalStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MaritalStatus), SalaryCardEmployeeCustomerIndividualMaritalStatus) ? EnumHelper.GetDescription((MaritalStatus)SalaryCardEmployeeCustomerIndividualMaritalStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string SalaryCardEmployeeCustomerAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string SalaryCardEmployeeCustomerAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string SalaryCardEmployeeCustomerAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string SalaryCardEmployeeCustomerAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string SalaryCardEmployeeCustomerAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string SalaryCardEmployeeCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string SalaryCardEmployeeCustomerAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string SalaryCardEmployeeCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Text Alerts Enabled?")]
        public bool SalaryCardEmployeeCustomerTextAlertsEnabled { get; set; }

        [DataMember]
        [Display(Name = "E-mail Alerts Enabled?")]
        public bool SalaryCardEmployeeCustomerEmailAlertsEnabled { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid SalaryCardEmployeeBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string SalaryCardEmployeeBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public Guid SalaryCardEmployeeDesignationId { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public string SalaryCardEmployeeDesignationDescription { get; set; }

        [DataMember]
        [Display(Name = "Department")]
        public Guid SalaryCardEmployeeDepartmentId { get; set; }

        [DataMember]
        [Display(Name = "Department")]
        public string SalaryCardEmployeeDepartmentDescription { get; set; }

        [DataMember]
        [Display(Name = "Employee Type ")]
        public Guid SalaryCardEmployeeEmployeeTypeId { get; set; }

        [DataMember]
        [Display(Name = "Employee Type G/L Account")]
        public Guid SalaryCardEmployeeEmployeeTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Employee Type")]
        public string SalaryCardEmployeeEmployeeTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Employee Type Category")]
        public int SalaryCardEmployeeEmployeeTypeCategory { get; set; }

        [DataMember]
        [Display(Name = "Employee Type Category")]
        public string SalaryCardEmployeeEmployeeTypeCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeCategory), SalaryCardEmployeeEmployeeTypeCategory) ? EnumHelper.GetDescription((EmployeeCategory)SalaryCardEmployeeEmployeeTypeCategory) : string.Empty;
            }
        }
        
        [DataMember]
        [Display(Name = "PIN")]
        public string SalaryCardEmployeeCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "NSSF Number")]
        public string SalaryCardEmployeeNationalSocialSecurityFundNumber { get; set; }

        [DataMember]
        [Display(Name = "NHIF Number")]
        public string SalaryCardEmployeeNationalHospitalInsuranceFundNumber { get; set; }

        [DataMember]
        [Display(Name = "Blood Group")]
        public int SalaryCardEmployeeBloodGroup { get; set; }

        [DataMember]
        [Display(Name = "Blood Group")]
        public string SalaryCardEmployeeBloodGroupDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BloodGroup), SalaryCardEmployeeBloodGroup) ? EnumHelper.GetDescription((BloodGroup)SalaryCardEmployeeBloodGroup) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(PaySlipStatus), Status) ? EnumHelper.GetDescription((PaySlipStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }

        HashSet<PaySlipEntryDTO> _paySlipEntries;
        [DataMember]
        [Display(Name = "Pay Slip Entries")]
        public virtual ICollection<PaySlipEntryDTO> PaySlipEntries
        {
            get
            {
                if (_paySlipEntries == null)
                {
                    _paySlipEntries = new HashSet<PaySlipEntryDTO>();
                }
                return _paySlipEntries;
            }
            private set
            {
                _paySlipEntries = new HashSet<PaySlipEntryDTO>(value);
            }
        }
    }
}
