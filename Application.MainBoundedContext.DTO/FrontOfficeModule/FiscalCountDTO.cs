using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Infrastructure.Crosscutting.Framework.Utils;
using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class FiscalCountDTO : BindingModelBase<FiscalCountDTO>
    {
        public FiscalCountDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Teller")]
        
        public Guid TellerId { get; set; }

        [DataMember]
        [Display(Name = "Teller")]
        public string TellerDescription { get; set; }

        [DataMember]
        [Display(Name = "Treasury")]
       
        public Guid TreasuryId { get; set; }

        [DataMember]
        [Display(Name = "Treasury")]
        public string TreasuryDescription { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        

        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]

        public Guid ChartOfAccountId { get; set; }

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
                return string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Primary Description")]
        public string PrimaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Secondary Description")]
        public string SecondaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Total Amount")]
        public string TotalAmount { get; set; }

        [DataMember]
        [Display(Name = "TransactionType")]
        public int TransactionType { get; set; }

        [DataMember]
        [Display(Name = "TransactionType")]
        public string TransactionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(TreasuryTransactionType), TransactionType) ? EnumHelper.GetDescription((TreasuryTransactionType)TransactionType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "One-Thousands")]
        public decimal DenominationOneThousandValue { get; set; }

        [DataMember]
        [Display(Name = "Five-Hundreds")]
        public decimal DenominationFiveHundredValue { get; set; }

        [DataMember]
        [Display(Name = "Two-Hundreds")]
        public decimal DenominationTwoHundredValue { get; set; }

        [DataMember]
        [Display(Name = "One-Hundreds")]
        public decimal DenominationOneHundredValue { get; set; }

        [DataMember]
        [Display(Name = "Fifties")]
        public decimal DenominationFiftyValue { get; set; }

        [DataMember]
        [Display(Name = "Fourties")]
        public decimal DenominationFourtyValue { get; set; }

        [DataMember]
        [Display(Name = "Twenties")]
        public decimal DenominationTwentyValue { get; set; }

        [DataMember]
        [Display(Name = "Tens")]
        public decimal DenominationTenValue { get; set; }

        [DataMember]
        [Display(Name = "Fives")]
        public decimal DenominationFiveValue { get; set; }

        [DataMember]
        [Display(Name = "Ones")]
        public decimal DenominationOneValue { get; set; }

        [DataMember]
        [Display(Name = "Fifty-Cents")]
        public decimal DenominationFiftyCentValue { get; set; }

        [DataMember]
        [Display(Name = "Total Value")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Transaction Code")]
        public int TransactionCode { get; set; }

        [DataMember]
        [Display(Name = "Transaction Code")]
        public string TransactionCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemTransactionCode), TransactionCode) ? EnumHelper.GetDescription((SystemTransactionCode)TransactionCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "System Trace Audit Number")]
        public string SystemTraceAuditNumber { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Destination Branch")]

        public Guid DestinationBranchId { get; set; }


        [DataMember]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public SavingsProductDTO SavingsProduct { get; set; }

        public TellerDTO Teller { get; set; }

    }
}
