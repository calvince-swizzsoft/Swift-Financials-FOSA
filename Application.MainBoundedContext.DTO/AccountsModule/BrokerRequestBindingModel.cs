using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class BrokerRequestBindingModel : BindingModelBase<BrokerRequestBindingModel>
    {
        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Transaction Type")]
        public byte TransactionType { get; set; }

        [DataMember]
        [Display(Name = "Transaction Code")]
        public string TransactionCode { get; set; }

        [DataMember]
        [Display(Name = "Unique Transaction Identifier")]
        public string UniqueTransactionIdentifier { get; set; }

        [DataMember]
        [Display(Name = "Callback Payload")]
        public string CallbackPayload { get; set; }

        [DataMember]
        [Display(Name = "Incoming Cipher-Text Payload")]
        public string IncomingCipherTextPayload { get; set; }

        [DataMember]
        [Display(Name = "Incoming Plain-Text Payload")]
        public string IncomingPlainTextPayload { get; set; }

        [DataMember]
        [Display(Name = "Outgoing Cipher-Text Payload")]
        public string OutgoingCipherTextPayload { get; set; }

        [DataMember]
        [Display(Name = "Outgoing Plain-Text Payload")]
        public string OutgoingPlainTextPayload { get; set; }

        [DataMember]
        [Display(Name = "IPN Enabled?")]
        public bool IPNEnabled { get; set; }

        [DataMember]
        [Display(Name = "IPN Status")]
        public byte IPNStatus { get; set; }

        [DataMember]
        [Display(Name = "IPN Response")]
        public string IPNResponse { get; set; }

        [DataMember]
        [Display(Name = "System Trace Audit Number")]
        public string SystemTraceAuditNumber { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public byte Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BrokerRequestStatus), Status) ? EnumHelper.GetDescription((BrokerRequestStatus)Status) : string.Empty;
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