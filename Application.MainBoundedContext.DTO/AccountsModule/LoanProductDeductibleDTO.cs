using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanProductDeductibleDTO : BindingModelBase<LoanProductDeductibleDTO>
    {
        public LoanProductDeductibleDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public Guid LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public int CustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string CustomerAccountTypeProductCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ProductCode), CustomerAccountTypeProductCode) ? EnumHelper.GetDescription((ProductCode)CustomerAccountTypeProductCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Product")]
        [ValidGuid]
        public Guid CustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public int CustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Name")]
        public string CustomerAccountTypeTargetProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public int? CustomerAccountTypeTargetProductLoanProductSection { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public string CustomerAccountTypeTargetProductLoanProductSectionDescription
        {
            get
            {
                return CustomerAccountTypeTargetProductLoanProductSection.HasValue ? Enum.IsDefined(typeof(LoanProductSection), CustomerAccountTypeTargetProductLoanProductSection.Value) ? EnumHelper.GetDescription((LoanProductSection)CustomerAccountTypeTargetProductLoanProductSection.Value) : string.Empty : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid CustomerAccountTypeTargetProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Code")]
        public int CustomerAccountTypeTargetProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Name")]
        public string CustomerAccountTypeTargetProductChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Receivable G/L Account")]
        public Guid CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Receivable G/L Account Code")]
        public int CustomerAccountTypeTargetProductInterestReceivableChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Receivable G/L Account Name")]
        public string CustomerAccountTypeTargetProductInterestReceivableChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Is Refundable?")]
        public bool CustomerAccountTypeTargetProductIsRefundable { get; set; }

        [DataMember]
        [Display(Name = "Is Default?")]
        public bool CustomerAccountTypeTargetProductIsDefault { get; set; }

        [DataMember]
        [Display(Name = "Charge Type")]
        public int ChargeType { get; set; }

        [DataMember]
        [Display(Name = "Charge Type")]
        public string ChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ChargeType) ? EnumHelper.GetDescription((ChargeType)ChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Percentage")]
        public double ChargePercentage { get; set; }

        [DataMember]
        [Display(Name = "Fixed Amount")]
        public decimal ChargeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Net Off Investment Balance?")]
        public bool NetOffInvestmentBalance { get; set; }

        [DataMember]
        [Display(Name = "Compute charge on Top-up?")]
        public bool ComputeChargeOnTopUp { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
