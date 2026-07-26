using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class GroupTextAlertDTO : BindingModelBase<GroupTextAlertDTO>
    {
        public GroupTextAlertDTO()
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
