
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class CustomerMessageHistoryDTO : BindingModelBase<CustomerMessageHistoryDTO>
    {
        public CustomerMessageHistoryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public int MessageCategory { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string MessageCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MessageCategory), MessageCategory) ? EnumHelper.GetDescription((MessageCategory)MessageCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [DataMember]
        [Display(Name = "Recipient")]
        public string Recipient { get; set; }

        [DataMember]
        [Display(Name = "Body")]
        public string Body { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
