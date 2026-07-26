using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeDisciplinaryCaseDTO : BindingModelBase<EmployeeDisciplinaryCaseDTO>
    {
        public EmployeeDisciplinaryCaseDTO()
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
        [Display(Name = "Customer")]
        public Guid EmployeeCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int EmployeeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string EmployeeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), EmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)EmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

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
        [Display(Name = "Identity Card Number")]
        public string EmployeeCustomerIndividualIdentityCardNumber { get; set; }

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
                return string.Format("{0} {1} {2}", EmployeeCustomerIndividualSalutationDescription, EmployeeCustomerIndividualFirstName, EmployeeCustomerIndividualLastName);
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
        [Display(Name = "Incident Date")]
        public DateTime IncidentDate { get; set; }

        [DataMember]
        [Display(Name = "Disciplinary Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Disciplinary Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeDisciplinaryType), Type) ? EnumHelper.GetDescription((EmployeeDisciplinaryType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Document")]
        public string FileName { get; set; }

        [DataMember]
        [Display(Name = "Title")]
        [Required]
        public string FileTitle { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        [Required]
        public string FileDescription { get; set; }

        [DataMember]
        [Display(Name = "MIME Type")]
        public string FileMIMEType { get; set; }

        [DataMember]
        [Display(Name = "Buffer")]
        public byte[] FileBuffer { get; set; }

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
