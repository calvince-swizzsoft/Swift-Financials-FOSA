using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CustomerAccountDTO : BindingModelBase<CustomerAccountDTO>
    {
        public CustomerAccountDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Employer")]
        public Guid CustomerStationZoneDivisionEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string CustomerStationZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public byte CustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerType)CustomerType);
            }
        }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public byte CustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation);
            }
        }

        [DataMember]
        [Display(Name = "Customer Gender")]
        public byte CustomerIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Customer Gender")]
        public string CustomerIndividualGenderDescription
        {
            get
            {
                return EnumHelper.GetDescription((Gender)CustomerIndividualGender);
            }
        }

        [DataMember]
        [Display(Name = "Customer Marital Status")]
        public byte CustomerIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Customer Marital Status")]
        public string CustomerIndividualMaritalStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((MaritalStatus)CustomerIndividualMaritalStatus);
            }
        }

        [DataMember]
        [Display(Name = "Customer Identity Card Type")]
        public byte CustomerIndividualIdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Customer Identity Card Type")]
        public string CustomerIndividualIdentityCardTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IdentityCardType)CustomerIndividualIdentityCardType);
            }
        }

        [DataMember]
        [Display(Name = "Customer Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Customer Nationality")]
        public byte CustomerIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Customer Nationality")]
        public string CustomerIndividualNationalityDescription
        {
            get
            {
                return EnumHelper.GetDescription((Nationality)CustomerIndividualNationality);
            }
        }

        [DataMember]
        [Display(Name = "Customer Employment Terms-Of-Service")]
        public byte? CustomerIndividualEmploymentTermsOfService { get; set; }

        [DataMember]
        [Display(Name = "Customer Employment Terms-Of-Service")]
        public string CustomerIndividualEmploymentTermsOfServiceDescription
        {
            get
            {
                return CustomerIndividualEmploymentTermsOfService.HasValue ? EnumHelper.GetDescription((TermsOfService)CustomerIndividualEmploymentTermsOfService.Value) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Customer Serial Number")]
        public string PaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Customer Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Individual Type")]
        public byte CustomerIndividualType { get; set; }

        [DataMember]
        [Display(Name = "Customer Individual Type")]
        public string CustomerIndividualTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IndividualType)CustomerIndividualType);
            }
        }

        [Display(Name = "Customer Classification")]
        public byte CustomerIndividualClassification { get; set; }

        [Display(Name = "Customer Classification")]
        public string CustomerIndividualClassificationDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerClassification)CustomerIndividualClassification);
            }
        }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string CustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string CustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? CustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Identification Number")]
        public string CustomerIdentificationNumber
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = CustomerIndividualIdentityCardNumber;
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerNonIndividualRegistrationNumber;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Is Defaulter?")]
        public bool CustomerIsDefaulter { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int BranchCode { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        public string BranchAddressCity { get; set; }

        [DataMember]
        public string BranchAddressStreet { get; set; }

        [DataMember]
        public string BranchAddressEmail { get; set; }

        [DataMember]
        public string BranchAddressLandLine { get; set; }

        [DataMember]
        public string BranchAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public Guid BranchCompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }

        [DataMember]
        public string BranchCompanyAddressCity { get; set; }

        [DataMember]
        public string BranchCompanyAddressStreet { get; set; }

        [DataMember]
        public string BranchCompanyAddressEmail { get; set; }

        [DataMember]
        public string BranchCompanyAddressLandLine { get; set; }

        [DataMember]
        public string BranchCompanyAddressMobileLine { get; set; }

        [DataMember]
        public string BranchCompanyRecoveryPriority { get; set; }

        [DataMember]
        public bool BranchCompanyEnforceInvestmentProductExemptions { get; set; }

        [DataMember]

        public int CustomerFilter { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string CustomerFilterDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerFilter), CustomerFilter) ? EnumHelper.GetDescription((CustomerFilter)CustomerFilter) : string.Empty;
            }
        }

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
        [Display(Name = "Rounding Type")]
        public int? CustomerAccountTypeTargetProductRoundingType { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public string CustomerAccountTypeTargetProductRoundingTypeDescription
        {
            get
            {
                return CustomerAccountTypeTargetProductRoundingType.HasValue ? Enum.IsDefined(typeof(RoundingType), CustomerAccountTypeTargetProductRoundingType.Value) ? EnumHelper.GetDescription((RoundingType)CustomerAccountTypeTargetProductRoundingType.Value) : string.Empty : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Recovery Priority")]
        public int CustomerAccountTypeTargetProductRecoveryPriority { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid CustomerAccountTypeTargetProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Type")]
        public int CustomerAccountTypeTargetProductChartOfAccountType { get; set; }

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
        [Display(Name = "Product Interest Receivable G/L Account Type")]
        public int CustomerAccountTypeTargetProductInterestReceivableChartOfAccountType { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Receivable G/L Account Code")]
        public int CustomerAccountTypeTargetProductInterestReceivableChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Receivable G/L Account Name")]
        public string CustomerAccountTypeTargetProductInterestReceivableChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Received G/L Account")]
        public Guid CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Received G/L Account Type")]
        public int CustomerAccountTypeTargetProductInterestReceivedChartOfAccountType { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Received G/L Account Code")]
        public int CustomerAccountTypeTargetProductInterestReceivedChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Received G/L Account Name")]
        public string CustomerAccountTypeTargetProductInterestReceivedChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Charged G/L Account")]
        public Guid CustomerAccountTypeTargetProductInterestChargedChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Charged G/L Account Type")]
        public int CustomerAccountTypeTargetProductInterestChargedChartOfAccountType { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Charged G/L Account Code")]
        public int CustomerAccountTypeTargetProductInterestChargedChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product Interest Charged G/L Account Name")]
        public string CustomerAccountTypeTargetProductInterestChargedChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Is Refundable?")]
        public bool CustomerAccountTypeTargetProductIsRefundable { get; set; }

        [DataMember]
        [Display(Name = "Maturity Period")]
        public int CustomerAccountTypeTargetProductMaturityPeriod { get; set; }

        [DataMember]
        [Display(Name = "Transfer balance to parent product on membership termination?")]
        public bool CustomerAccountTypeTargetProductTransferBalanceToParentOnMembershipTermination { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public Guid? CustomerAccountTypeTargetProductParentId { get; set; }

        [DataMember]
        [Display(Name = "Maximum Allowed Withdrawal")]
        public decimal CustomerAccountTypeTargetProductMaximumAllowedWithdrawal { get; set; }

        [DataMember]
        [Display(Name = "Maximum Allowed Deposit")]
        public decimal CustomerAccountTypeTargetProductMaximumAllowedDeposit { get; set; }

        [DataMember]
        [Display(Name = "Minimum Balance")]
        public decimal CustomerAccountTypeTargetProductMinimumBalance { get; set; }

        [DataMember]
        [Display(Name = "Is Default?")]
        public bool CustomerAccountTypeTargetProductIsDefault { get; set; }

        [DataMember]
        [Display(Name = "Is Microcredit?")]
        public bool CustomerAccountTypeTargetProductIsMicrocredit { get; set; }

        [DataMember]
        [Display(Name = "Charge Clearance Fee?")]
        public bool CustomerAccountTypeTargetProductChargeClearanceFee { get; set; }

        [DataMember]
        [Display(Name = "Withdrawal Notice Amount")]
        public decimal CustomerAccountTypeTargetProductWithdrawalNoticeAmount { get; set; }

        [DataMember]
        [Display(Name = "Withdrawal Notice Period")]
        public int CustomerAccountTypeTargetProductWithdrawalNoticePeriod { get; set; }

        [DataMember]
        [Display(Name = "Withdrawal Interval")]
        public int CustomerAccountTypeTargetProductWithdrawalInterval { get; set; }

        [DataMember]
        [Display(Name = "Throttle scheduled arrears recovery?")]
        public bool CustomerAccountTypeTargetProductThrottleScheduledArrearsRecovery { get; set; }

        [DataMember]
        [Display(Name = "Throttle Over-The-Counter Withdrawals?")]
        public bool CustomerAccountTypeTargetProductThrottleOverTheCounterWithdrawals { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string FullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            BranchCode.ToString().PadLeft(3, '0'),
                            CustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Scored Loan Disbursement Product Code")]
        public int ScoredLoanDisbursementProductCode { get; set; }

        [DataMember]
        [Display(Name = "Scored Loan Limit")]
        public decimal ScoredLoanLimit { get; set; }

        [DataMember]
        [Display(Name = "Scored Loan Limit Remarks")]
        public string ScoredLoanLimitRemarks { get; set; }

        [DataMember]
        [Display(Name = "Scored Loan Limit Date")]
        public DateTime? ScoredLoanLimitDate { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), Status) ? EnumHelper.GetDescription((CustomerAccountStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public int RecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), RecordStatus) ? EnumHelper.GetDescription((RecordStatus)RecordStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [DataMember]
        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataMember]
        [Display(Name = "Book Balance")]
        public decimal BookBalance { get; set; }

        [DataMember]
        [Display(Name = "Available Balance")]
        public decimal AvailableBalance { get; set; }

        [DataMember]
        [Display(Name = "Principal Balance")]
        public decimal PrincipalBalance { get; set; }

        [DataMember]
        [Display(Name = "Interest Balance")]
        public decimal InterestBalance { get; set; }

        [DataMember]
        [Display(Name = "Carry Forwards Balance")]
        public decimal CarryForwardsBalance { get; set; }

        [DataMember]
        [Display(Name = "Principal Arrearages Balance")]
        public decimal PrincipalArrearagesBalance { get; set; }

        [DataMember]
        [Display(Name = "Interest Arrearages Balance")]
        public decimal InterestArrearagesBalance { get; set; }

        [DataMember]
        [Display(Name = "Statement Type")]
        public int CustomerAccountStatementType { get; set; }

        [DataMember]
        [Display(Name = "Statement Type")]
        public string CustomerAccountStatementTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatementType), CustomerAccountStatementType) ? EnumHelper.GetDescription((CustomerAccountStatementType)CustomerAccountStatementType) : string.Empty;
            }
        }


        [DataMember]
        [Display(Name = "Action  Type")]
        public int CustomerAccountManagementAction { get; set; }

        [DataMember]
        [Display(Name = "Action Type")]
        public string CustomerAccountManagementActionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountManagementAction), CustomerAccountManagementAction) ? EnumHelper.GetDescription((CustomerAccountManagementAction)CustomerAccountManagementAction) : string.Empty;
            }
        }


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
        [Display(Name = "Amount Applied")]
        public decimal TotalValue { get; set; }

        public CustomerAccountDTO CustomerAccount { get; set; }
        public List<SavingsProductDTO> savingsProduct { get; set; }
        public SavingsProductDTO Savings { get; set; }
        public List<SavingsProductDTO> ProductDTOs { get; set; }
        public InvestmentProductDTO Investments { get; set; }
        public ObservableCollection<InvestmentProductDTO> investmentProducts { get; set; }
        public List<CustomerDTO> customerList { get; set; }

        public ObservableCollection<SavingsProductDTO> savingsProducts { get; set; }

        [DataMember]
        public string ErrorMessageResult { get; set; }


        // Getting and Setting values from the Registry Module
        [DataMember]
        public CustomerDTO Customers { get; set; }

        [DataMember]
        [Display(Name = " New Available Balance")]
        public decimal NewAvailableBalance { get; set; }


        [DataMember]
        public LoanProductDTO LoanProductsDTO { get; set; }




        // Additional DTOs
        // Additional DTOs
        [DataMember]
        [Display(Name = "Payment Per Period")]
        public double PaymentPerPeriod { get; set; }

        [DataMember]
        [Display(Name = "Number Of Periods")]
        public double NumberOfPeriods { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        public CashDepositRequestDTO CashDeposit { get; set; }
    }
}
