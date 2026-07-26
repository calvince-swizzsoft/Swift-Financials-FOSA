using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class EmailAlertDTO : BindingModelBase<EmailAlertDTO>
    {
        public EmailAlertDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid? BranchId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "From")]
        [Required]
        public string MailMessageFrom { get; set; }

        [DataMember]
        [Display(Name = "Recipient")]
        [Required]
        public string MailMessageTo { get; set; }

        [DataMember]
        [Display(Name = "CC")]
        public string MailMessageCC { get; set; }

        [DataMember]
        [Display(Name = "Subject")]
        [Required]
        public string MailMessageSubject { get; set; }

        [DataMember]
        [Display(Name = "Message")]
        [Required]
        public string MailMessageBody { get; set; }

        [DataMember]
        [Display(Name = "Message")]
        public string MaskedMailMessageBody { get; set; }

        [DataMember]
        [Display(Name = "Is Body Html?")]
        public bool MailMessageIsBodyHtml { get; set; }

        [DataMember]
        [Display(Name = "DLR Status")]
        public int MailMessageDLRStatus { get; set; }

        [DataMember]
        [Display(Name = "DLR Status")]
        public string MailMessageDLRStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(DLRStatus), MailMessageDLRStatus) ? EnumHelper.GetDescription((DLRStatus)MailMessageDLRStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Origin")]
        public int MailMessageOrigin { get; set; }

        [DataMember]
        [Display(Name = "Origin")]
        public string MailMessageOriginDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MessageOrigin), MailMessageOrigin) ? EnumHelper.GetDescription((MessageOrigin)MailMessageOrigin) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Priority")]
        public int MailMessagePriority { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public string MailMessagePriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), MailMessagePriority) ? EnumHelper.GetDescription((QueuePriority)MailMessagePriority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Send Retry")]
        public int MailMessageSendRetry { get; set; }

        [DataMember]
        [Display(Name = "Security Critical")]
        public bool MailMessageSecurityCritical { get; set; }

        [DataMember]
        [Display(Name = "Attachments")]
        public string MailMessageAttachments { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
