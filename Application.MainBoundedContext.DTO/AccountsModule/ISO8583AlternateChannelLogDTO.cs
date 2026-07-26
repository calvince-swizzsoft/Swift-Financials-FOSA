using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ISO8583AlternateChannelLogDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Alternate Channel Type")]
        public short AlternateChannelType { get; set; }

        [Display(Name = "Alternate Channel Type")]
        public string AlternateChannelTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AlternateChannelType), (int)AlternateChannelType) ? EnumHelper.GetDescription((AlternateChannelType)AlternateChannelType) : string.Empty;
            }
        }

        [Display(Name = "ISO8583 Message Type Identification")]
        public string ISO8583MessageTypeIdentification { get; set; }

        [Display(Name = "ISO8583 Primary Account Number")]
        public string ISO8583PrimaryAccountNumber { get; set; }

        [Display(Name = "ISO8583 System Trace Audit Number")]
        public string ISO8583SystemTraceAuditNumber { get; set; }

        [Display(Name = "ISO8583 Retrieval Reference Number")]
        public string ISO8583RetrievalReferenceNumber { get; set; }

        [Display(Name = "ISO8583 Message")]
        public string ISO8583Message { get; set; }

        [Display(Name = "ISO8583 Amount")]
        public decimal ISO8583Amount { get; set; }

        [Display(Name = "Response")]
        public string Response { get; set; }

        [Display(Name = "Is Reversed?")]
        public bool IsReversed { get; set; }

        [Display(Name = "Is Reconciled?")]
        public bool IsReconciled { get; set; }

        [Display(Name = "Reconciled By")]
        public string ReconciledBy { get; set; }

        [Display(Name = "System Trace Audit Number")]
        public string SystemTraceAuditNumber { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
