using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class LeaveApplicationBindingModel : BindingModelBase<LeaveApplicationBindingModel>
    {
        public LeaveApplicationBindingModel()
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
        [Display(Name = "First Name")]
        public string EmployeeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string EmployeeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Employee Name")]
        public string EmployeeCustomerFullName { get; set; }

        [DataMember]
        [Display(Name = "Leave Type")]
        [ValidGuid]
        public Guid LeaveTypeId { get; set; }
        [Display(Name = "Leave Type")]
        public string LeaveTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime DurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Reason for Leave")]
        [Required]
        public string Reason { get; set; }

        [DataMember]
        [Display(Name = "Balance(Days)")]
        public decimal Balance { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription { get; set; }
      

        [DataMember]
        [Display(Name = "Document #")]
        public string DocumentNumber { get; set; }

        [DataMember]
        [Display(Name = "Document")]
        public string FileName { get; set; }

        [DataMember]
        [Display(Name = "Title")]
        public string FileTitle { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        public string FileDescription { get; set; }

        [DataMember]
        [Display(Name = "MIME Type")]
        public string FileMIMEType { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected Date")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Recalled By")]
        public string RecalledBy { get; set; }

        [DataMember]
        [Display(Name = "Recall Remarks")]
        public string RecallRemarks { get; set; }

        [DataMember]
        [Display(Name = "Recalled Date")]
        public DateTime? RecalledDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}