using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CreditTypeCommissionDTO : BindingModelBase<CreditTypeCommissionDTO>
    {
        public CreditTypeCommissionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "CreditType")]
        public Guid CreditTypeId { get; set; }

        [DataMember]
        [Display(Name = "CreditType")]
        public CreditTypeDTO CreditType { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public CommissionDTO Commission { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
