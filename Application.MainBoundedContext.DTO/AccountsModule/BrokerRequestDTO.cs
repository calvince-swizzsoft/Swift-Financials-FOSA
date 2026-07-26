using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class BrokerRequestDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Transaction Type")]
        public byte TransactionType { get; set; }

        [Display(Name = "Transaction Code")]
        public string TransactionCode { get; set; }

        [Display(Name = "Unique Transaction Identifier")]
        public string UniqueTransactionIdentifier { get; set; }

        [Display(Name = "Callback Payload")]
        public string CallbackPayload { get; set; }

        [Display(Name = "Incoming Cipher-Text Payload")]
        public string IncomingCipherTextPayload { get; set; }

        [Display(Name = "Incoming Plain-Text Payload")]
        public string IncomingPlainTextPayload { get; set; }

        [Display(Name = "Outgoing Cipher-Text Payload")]
        public string OutgoingCipherTextPayload { get; set; }

        [Display(Name = "Outgoing Plain-Text Payload")]
        public string OutgoingPlainTextPayload { get; set; }

        [Display(Name = "IPN Enabled?")]
        public bool IPNEnabled { get; set; }

        [Display(Name = "IPN Status")]
        public byte IPNStatus { get; set; }

        [Display(Name = "IPN Response")]
        public string IPNResponse { get; set; }

        [Display(Name = "System Trace Audit Number")]
        public string SystemTraceAuditNumber { get; set; }

        [Display(Name = "Status")]
        public byte Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BrokerRequestStatus), Status) ? EnumHelper.GetDescription((BrokerRequestStatus)Status) : string.Empty;
            }
        }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}