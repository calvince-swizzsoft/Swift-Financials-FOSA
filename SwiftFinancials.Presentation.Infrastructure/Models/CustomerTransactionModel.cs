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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    [DataContract]
    public class CustomerTransactionModel : BindingModelBase<CustomerTransactionModel>
    {
        public CustomerTransactionModel()
        {
            ApportionmentWrapper = new ApportionmentWrapper();
            CustomerAccount = new CustomerAccountDTO();
            CashDepositRequest = new CashDepositRequestDTO();
            Teller = new TellerDTO();
            AddAllAttributeValidators();
        }

        [DataMember]
        public string Drawer { get; set; }


        [DataMember]
        public Guid CurrentTellerId { get; set; }

        [DataMember]
        public Guid ChequeType { get; set; }

        [DataMember]
        public string DrawerBank { get; set; }

        [DataMember]

        public string DrawerBankBranch { get; set; }


        [DataMember]
        public DateTime WriteDate { get; set; }

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
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Transaction amount must be greater than zero!")]
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
        [Display(Name = "Credit G/L Account Id")]
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
        [Display(Name = "Debit G/L Account Id")]
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

        Guid _creditCustomerAccountId;
        [DataMember]
        [Display(Name = "Credit Customer Account")]
        [ValidGuid]
        public Guid CreditCustomerAccountId
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
        [DataMember]
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

        Guid _debitCustomerAccountId;
        [DataMember]
        [Display(Name = "Debit Customer Account")]
        [ValidGuid]
        public Guid DebitCustomerAccountId
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

        [DataMember]
        public CustomerAccountDTO CustomerAccount { get; set; }

        [DataMember]
        public CashDepositRequestDTO CashDepositRequest { get; set; }

        [DataMember]
        public Guid CashDepositRequestId { get; set; }

        [DataMember]
        public int CashDepositCategory { get; set; }

        public string CashDepositCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CashDepositCategory), CashDepositCategory) ? EnumHelper.GetDescription((CashDepositCategory)CashDepositCategory) : string.Empty;
            }
        }

        [DataMember]
        public CashWithdrawalRequestDTO CashWithdrawal { get; set; }

        [DataMember]
        public Guid CashWithdrawalRequestId { get; set; }

        [DataMember]
        public int CashWithdrawalCategory { get; set; }

        public string CashWithdrawalCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CashWithdrawalCategory), CashWithdrawalCategory) ? EnumHelper.GetDescription((CashWithdrawalCategory)CashWithdrawalCategory) : string.Empty;
            }
        }

        [DataMember]
        public PaymentVoucherDTO PaymentVoucher { get; set; }

        [DataMember]
        public Guid PaymentVoucherId { get; set; }

        [DataMember]
        public string PaymentVoucherPayee { get; set; }

        [DataMember]
        public DateTime? PaymentVoucherWriteDate { get; set; }

        [DataMember]
        public Guid ChequeBookId { get; set; }

        [DataMember]
        public TellerDTO Teller { get; set; }

        [DataMember]
        public ExternalChequeDTO ChequeDeposit { get; set; }

        [DataMember]
        public PageCollectionInfo<GeneralLedgerTransaction> TellerStatements { get; set; }

        [DataMember]
        public CustomerDTO CustomerDTO { get; set; }

        [DataMember]
        public List<ExternalChequeDTO> CustomerAccountUnclearedCheques { get; set; }

        [DataMember]
        public List<Guid> ChequePayableCustomerAccountIds { get; set; }

        [DataMember]
        public string ChequePayableCustomerAccountIdsJson
        {
            get
            {
                return JsonConvert.SerializeObject(ChequePayableCustomerAccountIds);
            }
            set
            {
                ChequePayableCustomerAccountIds = JsonConvert.DeserializeObject<List<Guid>>(value);
            }
        }

        [DataMember]
        public List<CustomerAccountSignatoryDTO> CustomerAccountSignatories { get; set; }

        [DataMember]
        public List<ElectronicStatementOrderDTO> CustomerAccountMiniStatement { get; set; }

        [DataMember]
        public List<ApportionmentWrapper> Apportionments { get; set; }

        [DataMember]
        public ApportionmentWrapper ApportionmentWrapper { get; set; }



        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(FrontOfficeTransactionType), Type) ? EnumHelper.GetDescription((FrontOfficeTransactionType)Type) : string.Empty;
            }
        }

        [DataMember]
        public string DebitCustomerAccountJson
        {
            get
            {
                return JsonConvert.SerializeObject(DebitCustomerAccount);
            }
            set
            {
                DebitCustomerAccount = JsonConvert.DeserializeObject<CustomerAccountDTO>(value);
            }
        }

        [DataMember]
        public Boolean DialogResult { get; set; }

        [DataMember]
        public Guid BankAccountId { get; set; }

        //Added models

        [DataContract]
        public class CustomerReceiptBatchRequest
        {
            [DataMember]
            public Guid BranchId { get; set; }

            [DataMember]
            public decimal TotalValue { get; set; }

            [DataMember]
            public Guid BankAccountId { get; set; }

            [DataMember]
            public Guid PostingPeriodId { get; set; }

            [DataMember]
            public CustomerAccountDTO CustomerAccount { get; set; }

            [DataMember]
            public CustomerDTO CustomerDTO { get; set; }

            [DataMember]
            public string PrimaryDescription { get; set; }

            [DataMember]
            public string Reference { get; set; }
            public string _reference { get; set; }
            public DateTime? PostedDate { get; set; }
        }

        [DataContract]
        public class BatchCustomerReceiptRequest
        {
            [DataMember]
            public Guid BranchId { get; set; }

            [DataMember]
            public Guid BankAccountId { get; set; }

            [DataMember]
            public Guid PostingPeriodId { get; set; }

            [DataMember]
            public string PrimaryDescription { get; set; }

            [DataMember]
            public string Reference { get; set; }

            public string _reference { get; set; }

            [DataMember]
            public List<CustomerReceiptBatchRequest> Receipts { get; set; }

          
        }
    }
}