using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class ConditionalLendingDTO : BindingModelBase<ConditionalLendingDTO>
    {
        public ConditionalLendingDTO()
        {
            AddAllAttributeValidators();
        }

        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Loan Product")]
        public Guid LoanProductId { get; set; }

        [Display(Name = "Loan Product")]
        public string LoanProductDescription { get; set; }

        [Display(Name = "Name")]
        public string Description { get; set; }

        [Display(Name = "Is Locked")]
        public bool IsLocked { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }


        [DataMember]
        public CustomerDTO CustomerDTO { get; set; }

        public string ErrorMessageResult;

    }
}
