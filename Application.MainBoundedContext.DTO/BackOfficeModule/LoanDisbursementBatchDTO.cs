using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanDisbursementBatchDTO : BindingModelBase<LoanDisbursementBatchDTO>
    {
        public LoanDisbursementBatchDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [Display(Name = "Branch")]
        public string BranchCompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string BranchAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string BranchAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string BranchAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string BranchAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string BranchAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string BranchAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string BranchAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string BranchAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Data Period")]
        public Guid? DataAttachmentPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string DataAttachmentPeriodPostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Data Period")]
        public int DataAttachmentPeriodMonth { get; set; }

        [DataMember]
        [Display(Name = "Data Period")]
        public string DataAttachmentPeriodMonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), DataAttachmentPeriodMonth) ? string.Format("{0} - {1}", EnumHelper.GetDescription((Month)DataAttachmentPeriodMonth), DataAttachmentPeriodPostingPeriodDescription) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Data Period Remarks")]
        public string DataAttachmentPeriodRemarks { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int BatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedBatchNumber
        {
            get
            {
                return string.Format("{0}", BatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Batch Total")]
        public decimal BatchTotal { get; set; }
        
        [DataMember]
        [Display(Name = "Batch Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Batch Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(DisbursementType), Type) ? EnumHelper.GetDescription((DisbursementType)Type) : string.Empty;
            }
        }
        [DataMember]
        [Display(Name = "Auth Options")]
        public int Auth { get; set; }

        [DataMember]
        [Display(Name = "Auth Options")]
        public string AuthDescriptiom
        {
            get
            {
                return Enum.IsDefined(typeof(BatchAuthOption), Auth) ? EnumHelper.GetDescription((BatchAuthOption)Auth) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Category")]
        public int LoanProductCategory { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string LoanProductCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductCategory), LoanProductCategory) ? EnumHelper.GetDescription((LoanProductCategory)LoanProductCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public int Priority { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public string PriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), Priority) ? EnumHelper.GetDescription((QueuePriority)Priority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchStatus), Status) ? EnumHelper.GetDescription((BatchStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Verified/Rejected By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verification/Rejection Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected Date")]
        public DateTime? AuditedDate { get; set; }

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
        [Display(Name = "Posted Entries")]
        public string PostedEntries { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }


        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
    }
}
