using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class AlternateChannelReconciliationEntryDTO : BindingModelBase<AlternateChannelReconciliationEntryDTO>
    {
        public AlternateChannelReconciliationEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Alternate Channel Reconciliation Period")]
        [ValidGuid]
        public Guid AlternateChannelReconciliationPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Primary Account Number")]
        public string PrimaryAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "System Trace Audit Number")]
        public string SystemTraceAuditNumber { get; set; }

        [DataMember]
        [Display(Name = "Retrieval Reference Number")]
        public string RetrievalReferenceNumber { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((AlternateChannelReconciliationEntryStatus)Status);
            }
        }

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
