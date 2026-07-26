using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class TextAlertDTO : BindingModelBase<TextAlertDTO>
    {
        public TextAlertDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public Guid? BranchCompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Recipient")]
        [Required]
        public string TextMessageRecipient { get; set; }

        [DataMember]
        [Display(Name = "Message")]
        [Required]
        public string TextMessageBody { get; set; }

        [DataMember]
        [Display(Name = "Message")]
        public string MaskedTextMessageBody { get; set; }

        [DataMember]
        [Display(Name = "DLR Status")]
        public int TextMessageDLRStatus { get; set; }

        [DataMember]
        [Display(Name = "DLR Status")]
        public string TextMessageDLRStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(DLRStatus), TextMessageDLRStatus) ? EnumHelper.GetDescription((DLRStatus)TextMessageDLRStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Reference")]
        public string TextMessageReference { get; set; }

        [DataMember]
        [Display(Name = "Origin")]
        public int TextMessageOrigin { get; set; }

        [DataMember]
        [Display(Name = "Origin")]
        public string TextMessageOriginDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MessageOrigin), TextMessageOrigin) ? EnumHelper.GetDescription((MessageOrigin)TextMessageOrigin) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Priority")]
        public int TextMessagePriority { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public string TextMessagePriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), TextMessagePriority) ? EnumHelper.GetDescription((QueuePriority)TextMessagePriority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Send Retry")]
        public int TextMessageSendRetry { get; set; }

        [DataMember]
        [Display(Name = "Security Critical")]
        public bool TextMessageSecurityCritical { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public bool AppendSignature { get; set; }

        [DataMember]
        public int MessageCategory { get; set; }
    }
}
