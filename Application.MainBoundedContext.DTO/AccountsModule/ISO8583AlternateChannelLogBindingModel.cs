using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ISO8583AlternateChannelLogBindingModel : BindingModelBase<ISO8583AlternateChannelLogBindingModel>
    {
        public ISO8583AlternateChannelLogBindingModel()
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
        [Display(Name = "ISO8583 Message Type Identification")]
        public string ISO8583MessageTypeIdentification { get; set; }

        [DataMember]
        [Display(Name = "ISO8583 Primary Account Number")]
        public string ISO8583PrimaryAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "ISO8583 System Trace Audit Number")]
        public string ISO8583SystemTraceAuditNumber { get; set; }

        [DataMember]
        [Display(Name = "ISO8583 Retrieval Reference Number")]
        public string ISO8583RetrievalReferenceNumber { get; set; }

        [DataMember]
        [Display(Name = "ISO8583 Message")]
        public string ISO8583Message { get; set; }

        [DataMember]
        [Display(Name = "ISO8583 Amount")]
        public decimal ISO8583Amount { get; set; }
        
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
