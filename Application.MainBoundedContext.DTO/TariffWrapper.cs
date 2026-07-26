using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class TariffWrapper
    {
        [DataMember]
        [Display(Name = "Credit G/L Account")]
        public Guid CreditGLAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account Code")]
        public int CreditGLAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account Name")]
        public string CreditGLAccountName { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account")]
        public Guid DebitGLAccountId { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Code")]
        public int DebitGLAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Name")]
        public string DebitGLAccountName { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Borne By")]
        public int ChargeBenefactor { get; set; }

        [DataMember]
        [Display(Name = "Borne By")]
        public string ChargeBenefactorDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeBenefactor), ChargeBenefactor) ? EnumHelper.GetDescription((ChargeBenefactor)ChargeBenefactor) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Credit Customer Account")]
        public CustomerAccountDTO CreditCustomerAccount { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account")]
        public CustomerAccountDTO DebitCustomerAccount { get; set; }

        [DataMember]
        [Display(Name = "Indefinite Charge")]
        public DynamicChargeDTO DynamicCharge { get; set; }
    }
}
