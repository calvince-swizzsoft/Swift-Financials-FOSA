using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeDTO : BindingModelBase<EmployeeDTO>
    {
        public EmployeeDTO()
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
        [Display(Name = "Customer")]
        public CustomerDTO Customer { get; set; }

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
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

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
        public string CustomerFullName { get; set; }

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
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid email address!")]
        public string CustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string CustomerAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        [RegularExpression(@"^\+(?:[0-9]??){6,14}[0-9]$", ErrorMessage = "The mobile number should start with a plus sign, followed by the country code and national number")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "P.I.N")]
        public string CustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int BranchCode { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public Guid BranchCompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Company Localize Online Notifications?")]
        public bool BranchCompanyLocalizeOnlineNotifications { get; set; }

        [DataMember]
        [Display(Name = "Fingerprint Biometric Threshold")]
        public int BranchCompanyFingerprintBiometricThreshold { get; set; }

        [DataMember]
        [Display(Name = "System Initialization Time")]
        public TimeSpan BranchCompanyTimeDurationStartTime { get; set; }

        [DataMember]
        [Display(Name = "System Lock Time")]
        public TimeSpan BranchCompanyTimeDurationEndTime { get; set; }

        [DataMember]
        [Display(Name = "System Lock Time")]
        public bool BranchCompanyEnforceSystemLock { get; set; }

        [DataMember]
        [Display(Name = "Enforce Single User Session?")]
        public bool BranchCompanyEnforceSingleUserSession { get; set; }

        [DataMember]
        [Display(Name = "Enforce Mobile To Bank Reconciliation Verification?")]
        public bool BranchCompanyEnforceMobileToBankReconciliationVerification { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        [ValidGuid]
        public Guid DesignationId { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public string DesignationDescription { get; set; }

        [DataMember]
        [Display(Name = "Department")]
        [ValidGuid]
        public Guid DepartmentId { get; set; }

        [DataMember]
        [Display(Name = "Department")]
        public string DepartmentDescription { get; set; }

        [DataMember]
        [Display(Name = "Employee Type")]
        [ValidGuid]
        public Guid EmployeeTypeId { get; set; }

        [DataMember]
        [Display(Name = "Employee Type G/L Account")]
        public Guid EmployeeTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Employee Type")]
        public string EmployeeTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Employee Type Category")]
        public int EmployeeTypeCategory { get; set; }

        [DataMember]
        [Display(Name = "Employee Type Category")]
        public string EmployeeTypeCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeCategory), EmployeeTypeCategory) ? EnumHelper.GetDescription((EmployeeCategory)EmployeeTypeCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "N.S.S.F Number")]
        [Required]
        public string NationalSocialSecurityFundNumber { get; set; }

        [DataMember]
        [Display(Name = "N.H.I.F Number")]
        [Required]
        public string NationalHospitalInsuranceFundNumber { get; set; }

        [DataMember]
        [Display(Name = "Blood Group")]
        public int BloodGroup { get; set; }

        [DataMember]
        [Display(Name = "Blood Group")]
        public string BloodGroupDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BloodGroup), BloodGroup) ? EnumHelper.GetDescription((BloodGroup)BloodGroup) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Online Notifications Enabled?")]
        public bool OnlineNotificationsEnabled { get; set; }

        [DataMember]
        [Display(Name = "Enforce Biometrics For Login?")]
        public bool EnforceBiometricsForLogin { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Comment")]
        public string ApplicationComment { get; set; }

        [DataMember]
        [Display(Name = "User Name")]
        public string ApplicationUserName { get; set; }

        [DataMember]
        [Display(Name = "Roles")]
        public string ApplicationRoles { get; set; }

        [DataMember]
        [Display(Name = "Application E-mail")]
        public string ApplicationEmail { get; set; }

        [DataMember]
        [Display(Name = "Password Question")]
        public string ApplicationPasswordQuestion { get; set; }

        [DataMember]
        [Display(Name = "Is Approved?")]
        public bool ApplicationIsApproved { get; set; }

        [DataMember]
        [Display(Name = "Is Locked Out?")]
        public bool ApplicationIsLockedOut { get; set; }

        [DataMember]
        [Display(Name = "Is Online?")]
        public bool ApplicationIsOnline { get; set; }

        [DataMember]
        [Display(Name = "Creation Date")]
        public DateTime ApplicationCreationDate { get; set; }

        [DataMember]
        [Display(Name = "Last Activity Date")]
        public DateTime ApplicationLastActivityDate { get; set; }

        [DataMember]
        [Display(Name = "Last Lockout Date")]
        public DateTime ApplicationLastLockoutDate { get; set; }

        [DataMember]
        [Display(Name = "Last Login Date")]
        public DateTime ApplicationLastLoginDate { get; set; }

        [DataMember]
        [Display(Name = "Last Password Changed Date")]
        public DateTime ApplicationLastPasswordChangedDate { get; set; }

        [DataMember]
        [Display(Name = "Reset Password?")]
        public bool ApplicationResetPassword { get; set; }

        [DataMember]
        [Display(Name = "Web Services Url")]
        public string ApplicationWebServicesUrl { get; set; }

        [DataMember]
        [Display(Name = "SignalR Hub Url")]
        public string ApplicationSignalRHubUrl { get; set; }

        [DataMember]
        [Display(Name = "SSRS Host")]
        public string ApplicationSSRSHost { get; set; }

        [DataMember]
        [Display(Name = "SSRS Port")]
        public int? ApplicationSSRSPort { get; set; }

        [DataMember]
        [Display(Name = "SignalR Connection Id")]
        public string SignalRConnectionId { get; set; }

        [DataMember]
        [Display(Name = "Vendor Website")]
        public string ApplicationVendorWebsite { get; set; }

        [DataMember]
        [Display(Name = "Vendor Email")]
        public string ApplicationVendorEmail { get; set; }

        [DataMember]
        [Display(Name = "Vendor Telephone")]
        public string ApplicationVendorTelephone { get; set; }

        [DataMember]
        [Display(Name = "Data Source")]
        public string ApplicationDataSource { get; set; }

        [DataMember]
        [Display(Name = "Initial Catalog")]
        public string ApplicationInitialCatalog { get; set; }

        [DataMember]
        [Display(Name = "Server Date")]
        public DateTime ApplicationServerDate { get; set; }

        [DataMember]
        [Display(Name = "Password Expiry Period")]
        public int ApplicationPasswordExpiryPeriod { get; set; }

        [DataMember]
        [Display(Name = "Password History Policy")]
        public int ApplicationPasswordHistoryPolicy { get; set; }

        [DataMember]
        [Display(Name = "Env. User Name")]
        public string EnvironmentUserName { get; set; }

        [DataMember]
        [Display(Name = "Env. Machine Name")]
        public string EnvironmentMachineName { get; set; }

        [DataMember]
        [Display(Name = "Env. Domain Name")]
        public string EnvironmentDomainName { get; set; }

        [DataMember]
        [Display(Name = "Env. Operating System Version")]
        public string EnvironmentOSVersion { get; set; }

        [DataMember]
        [Display(Name = "Env. MAC Address")]
        public string EnvironmentMACAddress { get; set; }

        [DataMember]
        [Display(Name = "Env. Motherboard Serial Number")]
        public string EnvironmentMotherboardSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Env. Processor Id")]
        public string EnvironmentProcessorId { get; set; }

        [DataMember]
        [Display(Name = "Env. IP Address")]
        public string EnvironmentIPAddress { get; set; }

        // Additional Document DTO
        public Guid DocumentID { get; set; }  // Unique identifier for the document
        public byte[] PassportPhoto { get; set; }
        public byte[] SignaturePhoto { get; set; }
        public byte[] IDCardFrontPhoto { get; set; }
        public byte[] IDCardBackPhoto { get; set; }
        public string Referee { get; set; }

        public int EmployeeCustomerFilter { get; set; }

        [DataMember]
        [Display(Name = "Customer Product Code")]
        public string EmployeeCustomerFilterDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerFilter), EmployeeCustomerFilter) ? EnumHelper.GetDescription((CustomerFilter)EmployeeCustomerFilter) : string.Empty;
            }
        }

        public int EmployeeCustomerRecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Customer Record Status")]
        public string EmployeeCustomerRecordStatusDesccription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), EmployeeCustomerRecordStatus) ? EnumHelper.GetDescription((RecordStatus)EmployeeCustomerRecordStatus) : string.Empty;
            }
        }
    public string EmployeeDesignationDescription { get; set; }
        public string errormassage { get; set; }

    }
}
