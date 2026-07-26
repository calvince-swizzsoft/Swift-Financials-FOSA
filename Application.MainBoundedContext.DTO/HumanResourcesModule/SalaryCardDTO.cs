using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class SalaryCardDTO : BindingModelBase<SalaryCardDTO>
    {
        public SalaryCardDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        [ValidGuid]
        public Guid EmployeeId { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        public EmployeeDTO Employee { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid EmployeeCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int EmployeeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string EmployeeCustomerSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), EmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)EmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string EmployeeCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public int EmployeeCustomerIndividualIdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string EmployeeCustomerIndividualIdentityCardTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(IdentityCardType), EmployeeCustomerIndividualIdentityCardType) ? EnumHelper.GetDescription((IdentityCardType)EmployeeCustomerIndividualIdentityCardType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Nationality")]
        public int EmployeeCustomerIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string EmployeeCustomerIndividualNationalityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Nationality), EmployeeCustomerIndividualNationality) ? EnumHelper.GetDescription((Nationality)EmployeeCustomerIndividualNationality) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int EmployeeCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedEmployeeCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", EmployeeCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string EmployeeCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string EmployeeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string EmployeeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Employee Name")]
        public string EmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", EmployeeCustomerSalutationDescription, EmployeeCustomerIndividualFirstName, EmployeeCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public int EmployeeCustomerIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string EmployeeCustomerIndividualGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), EmployeeCustomerIndividualGender) ? EnumHelper.GetDescription((Gender)EmployeeCustomerIndividualGender) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public int EmployeeCustomerIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string EmployeeCustomerIndividualMaritalStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MaritalStatus), EmployeeCustomerIndividualMaritalStatus) ? EnumHelper.GetDescription((MaritalStatus)EmployeeCustomerIndividualMaritalStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string EmployeeCustomerAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string EmployeeCustomerAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string EmployeeCustomerAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string EmployeeCustomerAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string EmployeeCustomerAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string EmployeeCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string EmployeeCustomerAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string EmployeeCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid EmployeeBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string EmployeeBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public Guid EmployeeDesignationId { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public string EmployeeDesignationDescription { get; set; }

        [DataMember]
        [Display(Name = "Department")]
        public Guid EmployeeDepartmentId { get; set; }

        [DataMember]
        [Display(Name = "Department")]
        public string EmployeeDepartmentDescription { get; set; }

        [DataMember]
        [Display(Name = "Employee Type ")]
        public Guid EmployeeEmployeeTypeId { get; set; }

        [DataMember]
        [Display(Name = "Employee Type G/L Account")]
        public Guid EmployeeEmployeeTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Employee Type")]
        public string EmployeeEmployeeTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Employee Type Category")]
        public int EmployeeEmployeeTypeCategory { get; set; }

        [DataMember]
        [Display(Name = "Employee Type Category")]
        public string EmployeeEmployeeTypeCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeCategory), EmployeeEmployeeTypeCategory) ? EnumHelper.GetDescription((EmployeeCategory)EmployeeEmployeeTypeCategory) : string.Empty;
            }
        }
        
        [DataMember]
        [Display(Name = "PIN")]
        public string EmployeeCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "NSSF Number")]
        public string EmployeeNationalSocialSecurityFundNumber { get; set; }

        [DataMember]
        [Display(Name = "NHIF Number")]
        public string EmployeeNationalHospitalInsuranceFundNumber { get; set; }

        [DataMember]
        [Display(Name = "Blood Group")]
        public int EmployeeBloodGroup { get; set; }

        [DataMember]
        [Display(Name = "Blood Group")]
        public string EmployeeBloodGroupDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BloodGroup), EmployeeBloodGroup) ? EnumHelper.GetDescription((BloodGroup)EmployeeBloodGroup) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salary Group")]
        //[ValidGuid]
        public Guid SalaryGroupId { get; set; }

        [DataMember]
        [Display(Name = "Salary Group")]
        public string SalaryGroupDescription { get; set; }

        [DataMember]
        [Display(Name = "Card Number")]
        public int CardNumber { get; set; }

        [DataMember]
        [Display(Name = "Card Number")]
        public string PaddedCardNumber
        {
            get
            {
                return string.Format("{0}", CardNumber).PadLeft(6, '0');
            }
        }

        [DataMember]
        [Display(Name = "Is Tax Exempt?")]
        public bool IsTaxExempt { get; set; }

        [DataMember]
        [Display(Name = "Tax Exemption")]
        public decimal TaxExemption { get; set; }

        [DataMember]
        [Display(Name = "Insurance Relief Amount")]
        public decimal InsuranceReliefAmount { get; set; }

        [DataMember]
        [Display(Name = "Card Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Create dDate")]
        public DateTime CreatedDate { get; set; }

        // added permeters
        [DataMember]
        [Display(Name = "Card Value")]
        public int CardValue { get; set; }
        public SalaryGroupEntryDTO SalaryGroupEntry { get; set; }

    }
}
