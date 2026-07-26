using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SPARROWAlternateChannelLogBindingModel : BindingModelBase<SPARROWAlternateChannelLogBindingModel>
    {
        public SPARROWAlternateChannelLogBindingModel()
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
        [Display(Name = "SPARROW Message Type")]
        public string SPARROWMessageType { get; set; }

        [DataMember]
        [Display(Name = "SPARROW SRC IMD")]
        public string SPARROWSRCIMD { get; set; }

        [DataMember]
        [Display(Name = "SPARROW Device Id")]
        public string SPARROWDeviceId { get; set; }

        [DataMember]
        [Display(Name = "SPARROW Date")]
        public string SPARROWDate { get; set; }

        [DataMember]
        [Display(Name = "SPARROW Time")]
        public string SPARROWTime { get; set; }

        [DataMember]
        [Display(Name = "SPARROW Primary Account Number")]
        public string SPARROWCardNumber { get; set; }

        [DataMember]
        [Display(Name = "SPARROW Message")]
        public string SPARROWMessage { get; set; }

        [DataMember]
        [Display(Name = "SPARROW Amount")]
        public decimal SPARROWAmount { get; set; }
        
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
