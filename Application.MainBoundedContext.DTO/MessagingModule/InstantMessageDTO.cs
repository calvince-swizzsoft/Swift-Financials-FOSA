using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Application.Seedwork;
using System;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class InstantMessageDTO : BindingModelBase<InstantMessageDTO>
    {
        public InstantMessageDTO()
        {
            AddAllAttributeValidators();
        }

        [Display(Name = "Connection Id")]
        [DataMember]
        public string ConnectionId { get; set; }

        [Display(Name = "Sender")]
        [DataMember]
        public string Sender { get; set; }

        [Display(Name = "Full Name")]
        [DataMember]
        public string FullName { get; set; }

        [Display(Name = "Message")]
        [DataMember]
        public string Message { get; set; }

        [Display(Name = "Created Date")]
        [DataMember]
        public DateTime CreatedDate { get; set; }
    }
}
