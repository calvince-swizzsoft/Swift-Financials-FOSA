using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class LeaveApplicationDTO
    {
        
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Customer")]
        public Guid EmployeeCustomerId { get; set; }

        [Display(Name = "Salutation")]
        public byte EmployeeCustomerIndividualSalutation { get; set; }

        [Display(Name = "Salutation")]
        public string EmployeeCustomerSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), (int)EmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)EmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [Display(Name = "Identity Card Type")]
        public byte EmployeeCustomerIndividualIdentityCardType { get; set; }

        [Display(Name = "Identity Card Type")]
        public string EmployeeCustomerIndividualIdentityCardTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(IdentityCardType), (int)EmployeeCustomerIndividualIdentityCardType) ? EnumHelper.GetDescription((IdentityCardType)EmployeeCustomerIndividualIdentityCardType) : string.Empty;
            }
        }

        [Display(Name = "Identity Card Number")]
        public string EmployeeCustomerIndividualIdentityCardNumber { get; set; }

        [Display(Name = "Nationality")]
        public byte EmployeeCustomerIndividualNationality { get; set; }

        [Display(Name = "Nationality")]
        public string EmployeeCustomerIndividualNationalityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Nationality), (int)EmployeeCustomerIndividualNationality) ? EnumHelper.GetDescription((Nationality)EmployeeCustomerIndividualNationality) : string.Empty;
            }
        }

        [Display(Name = "Serial Number")]
        public int EmployeeCustomerSerialNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string PaddedEmployeeCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", EmployeeCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [Display(Name = "Payroll Numbers")]
        public string EmployeeCustomerIndividualPayrollNumbers { get; set; }

        [Display(Name = "First Name")]
        public string EmployeeCustomerIndividualFirstName { get; set; }

        [Display(Name = "Other Names")]
        public string EmployeeCustomerIndividualLastName { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", EmployeeCustomerSalutationDescription, EmployeeCustomerIndividualFirstName, EmployeeCustomerIndividualLastName);
            }
        }

        [Display(Name = "Gender")]
        public byte EmployeeCustomerIndividualGender { get; set; }

        [Display(Name = "Gender")]
        public string EmployeeCustomerIndividualGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), (int)EmployeeCustomerIndividualGender) ? EnumHelper.GetDescription((Gender)EmployeeCustomerIndividualGender) : string.Empty;
            }
        }

        [Display(Name = "Marital Status")]
        public byte EmployeeCustomerIndividualMaritalStatus { get; set; }

        [Display(Name = "Marital Status")]
        public string EmployeeCustomerIndividualMaritalStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MaritalStatus), (int)EmployeeCustomerIndividualMaritalStatus) ? EnumHelper.GetDescription((MaritalStatus)EmployeeCustomerIndividualMaritalStatus) : string.Empty;
            }
        }

        [Display(Name = "Address Line 1")]
        public string EmployeeCustomerAddressAddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string EmployeeCustomerAddressAddressLine2 { get; set; }

        [Display(Name = "Street")]
        public string EmployeeCustomerAddressStreet { get; set; }

        [Display(Name = "Postal Code")]
        public string EmployeeCustomerAddressPostalCode { get; set; }

        [Display(Name = "City")]
        public string EmployeeCustomerAddressCity { get; set; }

        [Display(Name = "E-mail")]
        public string EmployeeCustomerAddressEmail { get; set; }

        [Display(Name = "Land Line")]
        public string EmployeeCustomerAddressLandLine { get; set; }

        [Display(Name = "Mobile Line")]
        public string EmployeeCustomerAddressMobileLine { get; set; }

        [Display(Name = "Branch")]
        public Guid EmployeeBranchId { get; set; }

        [Display(Name = "Branch")]
        public string EmployeeBranchDescription { get; set; }

        [Display(Name = "Designation")]
        public Guid EmployeeDesignationId { get; set; }

        [Display(Name = "Designation")]
        public string EmployeeDesignationDescription { get; set; }

        [Display(Name = "Department")]
        public Guid EmployeeDepartmentId { get; set; }

        [Display(Name = "Department")]
        public string EmployeeDepartmentDescription { get; set; }

        [Display(Name = "Employee Type ")]
        public Guid EmployeeEmployeeTypeId { get; set; }

        [Display(Name = "Employee Type G/L Account")]
        public Guid EmployeeEmployeeTypeChartOfAccountId { get; set; }

        [Display(Name = "Employee Type")]
        public string EmployeeEmployeeTypeDescription { get; set; }

        [Display(Name = "Employee Type Category")]
        public byte EmployeeEmployeeTypeCategory { get; set; }

        [Display(Name = "Employee Type Category")]
        public string EmployeeEmployeeTypeCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeCategory), (int)EmployeeEmployeeTypeCategory) ? EnumHelper.GetDescription((EmployeeCategory)EmployeeEmployeeTypeCategory) : string.Empty;
            }
        }

        [Display(Name = "PIN")]
        public string EmployeeCustomerPersonalIdentificationNumber { get; set; }

        [Display(Name = "NSSF Number")]
        public string EmployeeNationalSocialSecurityFundNumber { get; set; }

        [Display(Name = "NHIF Number")]
        public string EmployeeNationalHospitalInsuranceFundNumber { get; set; }

        [Display(Name = "Blood Group")]
        public byte EmployeeBloodGroup { get; set; }

        [Display(Name = "Blood Group")]
        public string EmployeeBloodGroupDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BloodGroup), (int)EmployeeBloodGroup) ? EnumHelper.GetDescription((BloodGroup)EmployeeBloodGroup) : string.Empty;
            }
        }

        [Display(Name = "Leave Type")]
        public Guid LeaveTypeId { get; set; }

        [Display(Name = "Leave Type")]
        public LeaveTypeDTO LeaveType { get; set; }

        [Display(Name = "Leave Type")]
        public string LeaveTypeDescription { get; set; }

        [Display(Name = "Unit Type")]
        public byte LeaveTypeUnitType { get; set; }

        [Display(Name = "Unit Type")]
        public string LeaveTypeUnitTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LeaveUnitTypes), (int)LeaveTypeUnitType) ? EnumHelper.GetDescription((LeaveUnitTypes)LeaveTypeUnitType) : string.Empty;
            }
        }

        [Display(Name = "Is Accrued?")]
        public bool LeaveTypeIsAccrued { get; set; }

        [Display(Name = "Entitlement (Days)")]
        public int LeaveTypeEntitlement { get; set; }

        [Display(Name = "Exclude Holidays?")]
        public bool LeaveTypeExcludeHolidays { get; set; }

        [Display(Name = "Exclude Weekends?")]
        public bool LeaveTypeExcludeWeekends { get; set; }

        [Display(Name = "Start Date")]
        public DateTime DurationStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime DurationEndDate { get; set; }

        [Display(Name = "Reason for Leave")]
        public string Reason { get; set; }

        [Display(Name = "Balance (Days)")]
        public decimal Balance { get; set; }

        [Display(Name = "Status")]
        public byte Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LeaveApplicationStatus), (int)Status) ? EnumHelper.GetDescription((LeaveApplicationStatus)Status) : string.Empty;
            }
        }

        [Display(Name = "Document #")]
        public string DocumentNumber { get; set; }

        [Display(Name = "Document")]
        public string FileName { get; set; }

        [Display(Name = "Title")]
        public string FileTitle { get; set; }

        [Display(Name = "Description")]
        public string FileDescription { get; set; }

        [Display(Name = "MIME Type")]
        public string FileMIMEType { get; set; }

        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [Display(Name = "Authorized/Rejected Date")]
        public DateTime? AuthorizedDate { get; set; }

        [Display(Name = "Recalled By")]
        public string RecalledBy { get; set; }

        [Display(Name = "Recall Remarks")]
        public string RecallRemarks { get; set; }

        [Display(Name = "Recalled Date")]
        public DateTime? RecalledDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }


        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        // Additional DTOs
        [Display(Name = "Leave Auth Option")]
        public byte LeaveAutOption { get; set; }

        [Display(Name = "Status")]
        public string LeaveAutOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LeaveAuthOption), (int)LeaveAutOption) ? EnumHelper.GetDescription((LeaveAuthOption)LeaveAutOption) : string.Empty;
            }
        }

        public LeaveAuthOption LeaveAuthOption { get; set; }

        // List to hold error messages
        public string ErrorMessages { get; set; }

        // Property to check if there are any errors
        public bool HasErrors { get; set; }
        // Method to trigger validation for the DTO
        public void ValidateAll()
        {
            
        }

        // Custom validation for date duration (CheckDates)
        public static ValidationResult CheckDates(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as HolidayDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be HolidayDTO");

            if (bindingModel.DurationStartDate < bindingModel.PostingPeriodDurationStartDate || bindingModel.DurationStartDate > bindingModel.PostingPeriodDurationEndDate)
                return new ValidationResult("Start date is outside the valid range of the posting period.");
            else if (bindingModel.DurationEndDate < bindingModel.PostingPeriodDurationStartDate || bindingModel.DurationEndDate > bindingModel.PostingPeriodDurationEndDate)
                return new ValidationResult("End date is outside the valid range of the posting period.");
            else if (bindingModel.DurationStartDate > bindingModel.DurationEndDate)
                return new ValidationResult("Start date cannot be later than the end date.");

            return ValidationResult.Success;
        }

        [Display(Name = "Holiday Id")]
        public Guid holidayId { get; set; }  // Nullable if not every leave application is linked to a holiday

    }


}

