using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class TextAlertCommissionDTO : BindingModelBase<TextAlertCommissionDTO>
    {
        public TextAlertCommissionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "System Transaction Code")]
        public int SystemTransactionCode { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public CommissionDTO Commission { get; set; }
        
        [DataMember]
        [Display(Name = "Charge Benefactor")]
        public int ChargeBenefactor { get; set; }

        [DataMember]
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }
}
