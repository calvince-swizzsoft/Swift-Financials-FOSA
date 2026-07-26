using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SavingsProductCommissionDTO : BindingModelBase<SavingsProductCommissionDTO>
    {
        public SavingsProductCommissionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Savings Product")]
        public Guid SavingsProductId { get; set; }

        [DataMember]
        [Display(Name = "Savings Product")]
        public SavingsProductDTO SavingsProduct { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public CommissionDTO Commission { get; set; }

        [DataMember]
        [Display(Name = "Known Charge Type")]
        public int KnownChargeType { get; set; }

        [DataMember]
        [Display(Name = "Charge Benefactor")]
        public int ChargeBenefactor { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
