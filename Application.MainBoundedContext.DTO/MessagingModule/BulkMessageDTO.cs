using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
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
    public class BulkMessageDTO : BindingModelBase<BulkMessageDTO>
    {
        public BulkMessageDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Required]
        [StringLength(145)]
        [Display(Name = "Text Message")]
        [DataMember]
        public string TextMessage { get; set; }

        [Required]
        [DataMember]
        [Display(Name = "Recipients (comma separated phone numbers)")]
        public string Recipients { get; set; }

        [Display(Name = "Priority")]
        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        [Display(Name = "Append Signature?")]
        public bool AppendSignature { get; set; }

        [DataMember]
        [Display(Name = "Security Critical?")]
        public bool SecurityCritical { get; set; }
    }
}
