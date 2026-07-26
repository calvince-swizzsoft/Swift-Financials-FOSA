using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class BankReconciliationEntryDTO : BindingModelBase<BankReconciliationEntryDTO>
    {
        public BankReconciliationEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Bank Reconciliation Period")]
        [ValidGuid]
        public Guid BankReconciliationPeriodId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        public Guid? ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountName
        {
            get
            {
                if (ChartOfAccountId != null && ChartOfAccountId != Guid.Empty)
                    return string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName);
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Adjustment Type")]
        public int AdjustmentType { get; set; }

        [DataMember]
        [Display(Name = "Adjustment Type")]
        public string AdjustmentTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BankReconciliationAdjustmentType), AdjustmentType) ? EnumHelper.GetDescription((BankReconciliationAdjustmentType)AdjustmentType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        [DataMember]
        [Display(Name = "Cheque Number")]
        public string ChequeNumber { get; set; }

        [DataMember]
        [Display(Name = "Cheque Drawee")]
        public string ChequeDrawee { get; set; }

        [DataMember]
        [Display(Name = "Cheque Date")]
        public DateTime? ChequeDate { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
