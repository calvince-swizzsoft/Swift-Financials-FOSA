using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class QuickTextAlertDTO : BindingModelBase<QuickTextAlertDTO>
    {
        public QuickTextAlertDTO()
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
        public Guid BranchId { get; set; }

        [Required]
        [StringLength(145)]
        [Display(Name = "Text Message")]
        [DataMember]
        public string TextMessageBody { get; set; }

        [Required]
        [DataMember]
        [Display(Name = "Recipients")]
        public string Recipients { get; set; }

        [DataMember]
        [Display(Name = "Append Signature?")]
        public bool AppendSignature { get; set; }
    }
}
