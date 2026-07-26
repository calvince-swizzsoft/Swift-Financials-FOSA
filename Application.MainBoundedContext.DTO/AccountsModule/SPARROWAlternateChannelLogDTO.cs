using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SPARROWAlternateChannelLogDTO
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
