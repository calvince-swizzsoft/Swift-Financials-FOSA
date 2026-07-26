using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public class TransactionModel : BindingModelBase<TransactionModel>
    {
        public TransactionModel()
        {
            CustomerAccount = new CustomerAccountDTO();
            Teller = new TellerDTO();
            AddAllAttributeValidators();
        }


        Guid _branchId;
        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId
        {
            get { return _branchId; }
            set
            {
                if (_branchId != value)
                {
                    _branchId = value;
                    OnPropertyChanged(() => BranchId);
                }
            }
        }

        Guid _alternateChannelLogId;
        [Display(Name = "Alternate Channel Log")]
        public Guid AlternateChannelLogId
        {
            get { return _alternateChannelLogId; }
            set
            {
                if (_alternateChannelLogId != value)
                {
                    _alternateChannelLogId = value;
                    OnPropertyChanged(() => AlternateChannelLogId);
                }
            }
        }

        Guid _postingPeriodId;
        [DataMember]
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId
        {
            get { return _postingPeriodId; }
            set
            {
                if (_postingPeriodId != value)
                {
                    _postingPeriodId = value;
                    OnPropertyChanged(() => PostingPeriodId);
                }
            }
        }

        decimal _totalValue;
        [DataMember]
        [Display(Name = "Total Value")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Total Value must be greater than zero!")]
        public decimal TotalValue
        {
            get { return _totalValue; }
            set
            {
                if (_totalValue != value)
                {
                    _totalValue = value;
                    OnPropertyChanged(() => TotalValue);
                }
            }
        }

        decimal _creditAmount;
        [DataMember]
        [Display(Name = "Credit Amount")]
        public decimal CreditAmount
        {

            get { return _creditAmount; }

            set
            {
                if (_creditAmount != value)
                {
                    _creditAmount = value;
                    OnPropertyChanged(() => CreditAmount);
                }
            }
        }

        decimal _debitAmount;
        [DataMember]
        [Display(Name = "Debit Amount")]
        
        public decimal DebitAmount         {
            get { return _debitAmount; }
            set
            {
                if (_debitAmount != value)
                {
                    _debitAmount = value;
                    OnPropertyChanged(() => DebitAmount);
                }
            }
        }

        string _primaryDescription;
        [DataMember]
        [Display(Name = "Primary Description")]
        [Required]
        [StringLength(256)]
        public string PrimaryDescription
        {
            get { return _primaryDescription; }
            set
            {
                if (_primaryDescription != value)
                {
                    _primaryDescription = value;
                    OnPropertyChanged(() => PrimaryDescription);
                }
            }
        }

        string _secondaryDescription;
        [DataMember]
        [Display(Name = "Secondary Description")]
        [StringLength(256)]
        public string SecondaryDescription
        {
            get { return _secondaryDescription; }
            set
            {
                if (_secondaryDescription != value)
                {
                    _secondaryDescription = value;
                    OnPropertyChanged(() => SecondaryDescription);
                }
            }
        }

        string _reference;
        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        [StringLength(256)]
        public string Reference
        {
            get { return _reference; }
            set
            {
                if (_reference != value)
                {
                    _reference = value;
                    OnPropertyChanged(() => Reference);
                }
            }
        }

        int _moduleNavigationItemCode;
        [DataMember]
        [Display(Name = "Module Navigation Item Code")]
        public int ModuleNavigationItemCode
        {
            get { return _moduleNavigationItemCode; }
            set
            {
                if (_moduleNavigationItemCode != value)
                {
                    _moduleNavigationItemCode = value;
                    OnPropertyChanged(() => ModuleNavigationItemCode);
                }
            }
        }

        Guid _creditChartOfAccountId;
        [DataMember]
        [Display(Name = "Credit G/L Account")]
        [ValidGuid]
        public Guid CreditChartOfAccountId
        {
            get { return _creditChartOfAccountId; }
            set
            {
                if (_creditChartOfAccountId != value)
                {
                    _creditChartOfAccountId = value;
                    OnPropertyChanged(() => CreditChartOfAccountId);
                }
            }
        }

        Guid _debitChartOfAccountId;
        [DataMember]
        [Display(Name = "Debit G/L Account")]
        [ValidGuid]
        public Guid DebitChartOfAccountId
        {
            get { return _debitChartOfAccountId; }
            set
            {
                if (_debitChartOfAccountId != value)
                {
                    _debitChartOfAccountId = value;
                    OnPropertyChanged(() => DebitChartOfAccountId);
                }
            }
        }


        Guid _chartOfAccountId;
        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId
        {
            get { return _chartOfAccountId; }
            set
            {
                if (_chartOfAccountId != value)
                {
                    _chartOfAccountId = value;
                    OnPropertyChanged(() => ChartOfAccountId);
                }
            }
        }

        Guid _contraChartOfAccountId;
        [DataMember]
        [Display(Name = "Contra G/L Account")]
        [ValidGuid]
        public Guid ContraChartOfAccountId
        {
            get { return _contraChartOfAccountId; }
            set
            {
                if (_contraChartOfAccountId != value)
                {
                    _contraChartOfAccountId = value;
                    OnPropertyChanged(() => ContraChartOfAccountId);
                }
            }
        }

        Guid? _debitCustomerAccountId;
        [DataMember]
        [Display(Name = "Debit Customer Account")]
        public Guid? DebitCustomerAccountId
        {
            get { return _debitCustomerAccountId; }
            set
            {
                if (_debitCustomerAccountId != value)
                {
                    _debitCustomerAccountId = value;
                    OnPropertyChanged(() => DebitCustomerAccountId);
                }
            }
        }

        CustomerAccountDTO _debitCustomerAccount;
        [DataMember]
        [Display(Name = "Debit Customer Account")]
        public CustomerAccountDTO DebitCustomerAccount
        {
            get { return _debitCustomerAccount; }
            set
            {
                if (_debitCustomerAccount != value)
                {
                    _debitCustomerAccount = value;
                    OnPropertyChanged(() => DebitCustomerAccount);
                }
            }
        }

        public string DebitCustomerAccountJson
        {
            get
            {
                return DebitCustomerAccount == null ? null : JsonConvert.SerializeObject(DebitCustomerAccount);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DebitCustomerAccount = null;
                }
                else
                {
                    DebitCustomerAccount = JsonConvert.DeserializeObject<CustomerAccountDTO>(value);
                }
            }
        }

        [DataMember]
        Guid? _creditCustomerAccountId;
        [Display(Name = "Credit Customer Account")]
        public Guid? CreditCustomerAccountId
        {
            get { return _creditCustomerAccountId; }
            set
            {
                if (_creditCustomerAccountId != value)
                {
                    _creditCustomerAccountId = value;
                    OnPropertyChanged(() => CreditCustomerAccountId);
                }
            }
        }

        CustomerAccountDTO _creditCustomerAccount;
        [Display(Name = "Credit Customer Account")]
        public CustomerAccountDTO CreditCustomerAccount
        {
            get { return _creditCustomerAccount; }
            set
            {
                if (_creditCustomerAccount != value)
                {
                    _creditCustomerAccount = value;
                    OnPropertyChanged(() => CreditCustomerAccount);
                }
            }
        }

        int _transactionCode;
        [DataMember]
        [Display(Name = "Transaction Code")]
        public int TransactionCode
        {
            get { return _transactionCode; }
            set
            {
                if (_transactionCode != value)
                {
                    _transactionCode = value;
                    OnPropertyChanged(() => TransactionCode);
                }
            }
        }

        DateTime? _valueDate;
        [DataMember]
        [Display(Name = "Value Date")]
        public DateTime? ValueDate
        {
            get { return _valueDate; }
            set
            {
                if (_valueDate != value)
                {
                    _valueDate = value;
                    OnPropertyChanged(() => ValueDate);
                }
            }
        }

        //public List<TransactionModel> TransactionModels { get; set; }
        public FiscalCountDTO fiscalCountDTO { get; set; }


        [DataMember]
        public CustomerAccountDTO CustomerAccount { get; set; }

  
        [DataMember]
        public TellerDTO Teller { get; set; }

        public ApportionmentWrapper ApportionmentWrapper { get; set; }

        public List<ApportionmentWrapper> Apportionments { get; set; }

        public CustomerDTO CustomerDTO { get; set; }

        public List<ExternalChequeDTO> CustomerAccountUnclearedCheques { get; set; }

        public List<CustomerAccountSignatoryDTO> CustomerAccountSignatories { get; set; }

        public List<ElectronicStatementOrderDTO> CustomerAccountMiniStatement { get; set; }

        [DataMember]
        [Display(Name = "Available Balance")]
        public decimal AvailableBalance { get; set; }

        public int CustomerFilter { get; set; }

        [DataMember]
        [Display(Name = "CustomeR Product Code")]
        public string CustomerFilterDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerFilter), CustomerFilter) ? EnumHelper.GetDescription((CustomerFilter)CustomerFilter) : string.Empty;
            }
        }

        public int CustomerRecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Customer Record Status")]
        public string CustomerRecordStatusDesccription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), CustomerRecordStatus) ? EnumHelper.GetDescription((RecordStatus)CustomerRecordStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "JournalType")]
        public byte JournalType { get; set; }

        [DataMember]
        [Display(Name = "JournlType")]
        public string JournalTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(JournalVoucherType), (int)JournalType) ? EnumHelper.GetDescription((JournalVoucherType)JournalType) : string.Empty;
            }
        }

        


    }
}
