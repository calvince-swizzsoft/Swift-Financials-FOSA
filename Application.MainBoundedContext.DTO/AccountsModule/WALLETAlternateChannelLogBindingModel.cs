using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class WALLETAlternateChannelLogBindingModel : BindingModelBase<WALLETAlternateChannelLogBindingModel>
    {
        public WALLETAlternateChannelLogBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Alternate Channel Type")]
        public int AlternateChannelType { get; set; }

        [DataMember]
        [Display(Name = "Alternate Channel Type")]
        public string AlternateChannelTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AlternateChannelType), AlternateChannelType) ? EnumHelper.GetDescription((AlternateChannelType)AlternateChannelType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "WALLET Message Type Identification")]
        public string WALLETMessageTypeIdentification { get; set; }

        [DataMember]
        [Display(Name = "WALLET Primary Account Number")]
        public string WALLETPrimaryAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "WALLET System Trace Audit Number")]
        public string WALLETSystemTraceAuditNumber { get; set; }

        [DataMember]
        [Display(Name = "WALLET Retrieval Reference Number")]
        public string WALLETRetrievalReferenceNumber { get; set; }

        [DataMember]
        [Display(Name = "WALLET Message")]
        public string WALLETMessage { get; set; }

        [DataMember]
        [Display(Name = "WALLET CallbackPayload")]
        public string WALLETCallbackPayload { get; set; }

        [DataMember]
        [Display(Name = "WALLET Amount")]
        public decimal WALLETAmount { get; set; }
        
        [DataMember]
        [Display(Name = "Response")]
        public string Response { get; set; }

        [DataMember]
        [Display(Name = "Is Reversed?")]
        public bool IsReversed { get; set; }

        [DataMember]
        [Display(Name = "Is Reconciled?")]
        public bool IsReconciled { get; set; }

        [DataMember]
        [Display(Name = "Reconciled By")]
        public string ReconciledBy { get; set; }

        [DataMember]
        [Display(Name = "System Trace Audit Number")]
        public string SystemTraceAuditNumber { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
