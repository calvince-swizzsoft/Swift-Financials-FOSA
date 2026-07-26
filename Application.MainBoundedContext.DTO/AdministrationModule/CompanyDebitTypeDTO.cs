using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class CompanyDebitTypeDTO : BindingModelBase<CompanyDebitTypeDTO>
    {
        public CompanyDebitTypeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public CompanyDTO Company { get; set; }

        [DataMember]
        [Display(Name = "Debit Type")]
        public Guid DebitTypeId { get; set; }

        [DataMember]
        [Display(Name = "Debit Type")]
        public DebitTypeDTO DebitType { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
