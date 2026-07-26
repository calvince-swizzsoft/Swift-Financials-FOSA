using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class CustomerCreditTypeDTO : BindingModelBase<CustomerCreditTypeDTO>
    {
        public CustomerCreditTypeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public CustomerDTO Customer { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        public Guid CreditTypeId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        public CreditTypeDTO CreditType { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
