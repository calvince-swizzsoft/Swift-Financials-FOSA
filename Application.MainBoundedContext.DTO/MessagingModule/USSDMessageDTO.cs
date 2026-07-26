using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class USSDMessageDTO : BindingModelBase<USSDMessageDTO>
    {
        public USSDMessageDTO()
        {
            AddAllAttributeValidators();
        }

        [Required]
        [Display(Name = "Message")]
        [DataMember]
        public string Message { get; set; }
    }
}
