using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
//namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    [DataContract]
    public class CashTransferRequestDTO : BindingModelBase<CashTransferRequestDTO>
    {

        public CashTransferRequestDTO()
        {
            AddAllAttributeValidators();
        }

        [Display(Name = "TellerId")]
        public Guid TellerId { get; set; }

        [Display(Name = "Id")]
        public Guid Id { get; set; }

        //[Display(Name = "Employee")]
        //public Guid? EmployeeId { get; set; }

        Guid? _employeeId;
        [DataMember]
        [Display(Name = "Employee Id")]
        public Guid? EmployeeId
        {
            get { return _employeeId; }
            set
            {
                if (_employeeId != value)
                {
                    _employeeId = value;
                    OnPropertyChanged(() => EmployeeId);
                }
            }
        }


        [Display(Name = "Customer")]
        public Guid EmployeeCustomerId { get; set; }

        [Display(Name = "Salutation")]
        public byte EmployeeCustomerIndividualSalutation { get; set; }

        [Display(Name = "Salutation")]
        public string EmployeeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), (int)EmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)EmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [Display(Name = "First Name")]
        public string EmployeeCustomerIndividualFirstName { get; set; }

        [Display(Name = "Other Names")]
        public string EmployeeCustomerIndividualLastName { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", EmployeeCustomerIndividualSalutationDescription, EmployeeCustomerIndividualFirstName, EmployeeCustomerIndividualLastName);
            }
        }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CashTransferRequestStatus), Status) ? EnumHelper.GetDescription((CashTransferRequestStatus)Status) : string.Empty;
            }
        }

  
        [Display(Name = "Utilized")]
        public bool Utilized { get; set; }

        //[Display(Name = "Amount")]
        //public decimal Amount { get; set; }

        decimal _amount;
        [DataMember]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be greater than zero!")]
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(() => Amount);
                }
            }
        }

        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Acknowledged By")]
        public string AcknowledgedBy { get; set; }

        [Display(Name = "Acknowledged Date")]
        public DateTime? AcknowledgedDate { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "From")]
        public string CreatedBy { get; set; }

        //[DataMember]
        //[Display(Name = "TransactionType")]
        //public int TransactionType { get; set; }

        //[DataMember]
        //[Display(Name = "TransactionType")]
        //public string TransactionTypeDescription
        //{
        //    get
        //    {
        //        return Enum.IsDefined(typeof(TreasuryTransactionType), TransactionType) ? EnumHelper.GetDescription((TreasuryTransactionType)TransactionType) : string.Empty;
        //    }
        //}

        [DataMember]
        [Display(Name = "Cash Balance Status")]
        public int TellerCashBalanceStatusValue { get; set; } // Changed the name to avoid conflict

        [DataMember]
        [Display(Name = "Cash Balance Status")]
        public string TellerCashBalanceStatus
        {
            get
            {
                return Enum.IsDefined(typeof(TellerCashBalanceStatus), TellerCashBalanceStatusValue)
                    ? EnumHelper.GetDescription((TellerCashBalanceStatus)TellerCashBalanceStatusValue)
                    : string.Empty;
            }
        }


        //[Display(Name = "Opening Balance")]
        //public decimal OpeningBalance { get; set; }

        decimal _openingBalance;
        [DataMember]
        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance
        {
            get { return _openingBalance; }
            set
            {
                if (_openingBalance != value)
                {
                    _openingBalance = value;
                    OnPropertyChanged(() => OpeningBalance);
                }
            }
        }

        //[Display(Name = "Closing Balance")]
        //public decimal ClosingBalance { get; set; }

        decimal _closingBalance;
        [DataMember]
        [Display(Name = "Closing Balance")]
        //[RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Closing Balance must be greater than zero!")]
        public decimal ClosingBalance
        {
            get { return _closingBalance; }
            set
            {
                if (_closingBalance != value)
                {
                    _closingBalance = value;
                    OnPropertyChanged(() => ClosingBalance);
                }
            }
        }

        //[Display(Name = "Total Payments")]
        //public decimal TotalDebits { get; set; }

        decimal _totalCredits;
        [DataMember]
        [Display(Name = "Total Payments")]
        //[RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Total Payments must be greater than zero!")]
        public decimal TotalCredits
        {
            get { return _totalCredits; }
            set
            {
                if (_totalCredits != value)
                {
                    _totalCredits = value;
                    OnPropertyChanged(() => TotalCredits);
                }
            }
        }

        //[Display(Name = "Total Receipts")]
        //public decimal TotalDebits { get; set; }

        decimal _totalDebits;
        [DataMember]
        [Display(Name = "Total Debits")]
        //[RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Total Debits must be greater than zero!")]
        public decimal TotalDebits
        {
            get { return _totalDebits; }
            set
            {
                if (_totalDebits != value)
                {
                    _totalDebits = value;
                    OnPropertyChanged(() => TotalDebits);
                }
            }
        }

        //[Display(Name = "Expected Cash")]
        //public decimal BookBalance { get; set; }

        decimal _bookBalance;
        [DataMember]
        [Display(Name = "Expected Cash")]
        //[RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Total Value must be greater than zero!")]
        public decimal BookBalance
        {
            get { return _bookBalance; }
            set
            {
                if (_bookBalance != value)
                {
                    _bookBalance = value;
                    OnPropertyChanged(() => BookBalance);
                }
            }
        }

        //[Display(Name = "Cheques Pensing Transfer")]
        //public decimal UntransferredChequesValue { get; set; }

        decimal _untransferredChequesValue;
        [DataMember]
        [Display(Name = "Cheques Pending Transfer")]
        //[RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Total Value must be greater than zero!")]
        public decimal UntransferredChequesValue
        {
            get { return _untransferredChequesValue; }
            set
            {
                if (_untransferredChequesValue != value)
                {
                    _untransferredChequesValue = value;
                    OnPropertyChanged(() => UntransferredChequesValue);
                }
            }
        }

        [DataMember]
        [Display(Name = "Closing Balance Status")]
        public string ClosingBalanceStatus { get; set; }


    }
}
