using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CreditTypeDirectDebitDTO : BindingModelBase<CreditTypeDirectDebitDTO>
    {
        public CreditTypeDirectDebitDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        public Guid CreditTypeId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        public CreditTypeDTO CreditType { get; set; }

        [DataMember]
        [Display(Name = "Direct Debit")]
        public Guid DirectDebitId { get; set; }

        [DataMember]
        [Display(Name = "Direct Debit")]
        public DirectDebitDTO DirectDebit { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
