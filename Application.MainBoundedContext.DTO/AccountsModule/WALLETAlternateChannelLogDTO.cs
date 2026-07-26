using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class WALLETAlternateChannelLogDTO
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
