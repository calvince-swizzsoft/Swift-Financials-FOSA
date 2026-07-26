using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class QuickEmailAlertDTO : BindingModelBase<QuickEmailAlertDTO>
    {
        public QuickEmailAlertDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Company")]
        public Guid? CompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string CompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "From")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid sender email address!")]
        public string MailMessageFrom { get; set; }

        [DataMember]
        [Display(Name = "Subject")]
        [Required]
        public string MailMessageSubject { get; set; }

        [DataMember]
        [Display(Name = "Body")]
        [Required]
        public string MailMessageBody { get; set; }

        [DataMember]
        [Display(Name = "Is Body Html?")]
        public bool MailMessageIsBodyHtml { get; set; }

        [Required]
        [DataMember]
        [Display(Name = "Recipients")]
        public string Recipients { get; set; }
    }
}
