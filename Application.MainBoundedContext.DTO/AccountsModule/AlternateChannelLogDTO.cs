using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class AlternateChannelLogDTO
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
                return EnumHelper.GetDescription((AlternateChannelType)AlternateChannelType);
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

        [Display(Name = "SPARROW Message Type")]
        public string SPARROWMessageType { get; set; }

        [Display(Name = "SPARROW SRC IMD")]
        public string SPARROWSRCIMD { get; set; }

        [Display(Name = "SPARROW Device Id")]
        public string SPARROWDeviceId { get; set; }

        [Display(Name = "SPARROW Date")]
        public string SPARROWDate { get; set; }

        [Display(Name = "SPARROW Time")]
        public string SPARROWTime { get; set; }

        [Display(Name = "SPARROW Primary Account Number")]
        public string SPARROWCardNumber { get; set; }

        [Display(Name = "SPARROW Message")]
        public string SPARROWMessage { get; set; }

        [Display(Name = "SPARROW Amount")]
        public decimal SPARROWAmount { get; set; }

        [Display(Name = "WALLET Message Type Identification")]
        public string WALLETMessageTypeIdentification { get; set; }

        [Display(Name = "WALLET Primary Account Number")]
        public string WALLETPrimaryAccountNumber { get; set; }

        [Display(Name = "WALLET System Trace Audit Number")]
        public string WALLETSystemTraceAuditNumber { get; set; }

        [Display(Name = "WALLET Retrieval Reference Number")]
        public string WALLETRetrievalReferenceNumber { get; set; }

        [Display(Name = "WALLET Message")]
        public string WALLETMessage { get; set; }

        [Display(Name = "WALLET Callback Payload")]
        public string WALLETCallbackPayload { get; set; }

        [Display(Name = "WALLET Amount")]
        public decimal WALLETAmount { get; set; }

        [Display(Name = "WALLET Request Identifier")]
        public string WALLETRequestIdentifier { get; set; }

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
