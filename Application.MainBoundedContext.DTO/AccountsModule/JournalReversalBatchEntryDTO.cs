using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalReversalBatchEntryDTO : BindingModelBase<JournalReversalBatchEntryDTO>
    {
        public JournalReversalBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Journal Reversal Batch")]
        [ValidGuid]
        public Guid JournalReversalBatchId { get; set; }

        [DataMember]
        [Display(Name = "Journal Reversal Batch Priority")]
        public int JournalReversalBatchPriority { get; set; }

        [DataMember]
        [Display(Name = "Journal Reversal Batch Remarks")]
        public string JournalReversalBatchRemarks { get; set; }

        [DataMember]
        [Display(Name = "Journal Reversal Batch Authorized/Rejected By")]
        public string JournalReversalBatchAuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Journal Reversal Batch Number")]
        public int JournalReversalBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Journal Reversal Batch Number")]
        public string PaddedJournalReversalBatchNumber
        {
            get
            {
                return string.Format("{0}", JournalReversalBatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Journal")]
        [ValidGuid]
        public Guid JournalId { get; set; }

        [DataMember]
        [Display(Name = "Journal")]
        public JournalDTO Journal { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
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
                return Enum.IsDefined(typeof(BatchEntryStatus), Status) ? EnumHelper.GetDescription((BatchEntryStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
