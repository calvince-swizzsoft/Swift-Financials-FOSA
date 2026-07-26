using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeExitDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        #region Employee

        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Customer")]
        public Guid EmployeeCustomerId { get; set; }

        [Display(Name = "Salutation")]
        public int EmployeeCustomerIndividualSalutation { get; set; }

        [Display(Name = "Salutation")]
        public string EmployeeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), EmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)EmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [Display(Name = "First Name")]
        public string EmployeeCustomerIndividualFirstName { get; set; }

        [Display(Name = "Other Names")]
        public string EmployeeCustomerIndividualLastName { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", EmployeeCustomerIndividualSalutationDescription, EmployeeCustomerIndividualFirstName, EmployeeCustomerIndividualLastName);
            }
        }

        [Display(Name = "Identity Card Number")]
        public string EmployeeCustomerIndividualIdentityCardNumber { get; set; }

        [Display(Name = "Payroll")]
        public string EmployeeCustomerIndividualPayrollNumbers { get; set; }

        [Display(Name = "Serial Number")]
        public int EmployeeCustomerSerialNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string SerialNumber
        {
            get
            {
                return string.Format("{0}", EmployeeCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [Display(Name = "Branch")]
        public string EmployeeBranchDescription { get; set; }

        [Display(Name = "Designation")]
        public string EmployeeDesignationDescription { get; set; }

        [Display(Name = "Department")]
        public string EmployeeDepartmentDescription { get; set; }

        #endregion

        [Display(Name = "Reason")]
        public string Reason { get; set; }

        [Display(Name = "Type")]
        public int Type { get; set; }

        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeExitType), Type) ? EnumHelper.GetDescription((EmployeeExitType)Type) : string.Empty;
            }
        }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeExitStatus), Status) ? EnumHelper.GetDescription((EmployeeExitStatus)Status) : string.Empty;
            }
        }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Verified/Rejected By")]
        public string AuditedBy { get; set; }

        [Display(Name = "Verification/Rejection Remarks")]
        public string AuditRemarks { get; set; }

        [Display(Name = "Verified/Rejected Date")]
        public DateTime? AuditedDate { get; set; }

        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [Display(Name = "Authorized/Rejected Date")]
        public DateTime? AuthorizedDate { get; set; }

        [Display(Name = "Document")]
        public string FileName { get; set; }

        [Display(Name = "Title")]
        public string FileTitle { get; set; }

        [Display(Name = "Description")]
        public string FileDescription { get; set; }

        [Display(Name = "MIME Type")]
        public string FileMIMEType { get; set; }

        [Display(Name = "Buffer")]
        public byte[] FileBuffer { get; set; }
    }
}
