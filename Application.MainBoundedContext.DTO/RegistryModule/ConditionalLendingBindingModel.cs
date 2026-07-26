using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class ConditionalLendingBindingModel : BindingModelBase<ConditionalLendingBindingModel>
    {
        public ConditionalLendingBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        [ValidGuid]
        public Guid LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public string LoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Is Locked")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
