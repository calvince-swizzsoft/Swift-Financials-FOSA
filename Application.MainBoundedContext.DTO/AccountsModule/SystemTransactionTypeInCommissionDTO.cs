using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SystemTransactionTypeInCommissionDTO : BindingModelBase<SystemTransactionTypeInCommissionDTO>
    {
        public SystemTransactionTypeInCommissionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "System Transaction Type")]
        public int SystemTransactionType { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public CommissionDTO Commission { get; set; }

        [DataMember]
        [Display(Name = "Complement Type")]
        public int ComplementType { get; set; }

        [DataMember]
        [Display(Name = "Complement Type")]
        public string ComplementTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ComplementType) ? EnumHelper.GetDescription((ChargeType)ComplementType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Complement Percentage")]
        public double ComplementPercentage { get; set; }

        [DataMember]
        [Display(Name = "Complement Fixed Amount")]
        public decimal ComplementFixedAmount { get; set; }
        
        [DataMember]
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }
}
