using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanDisbursementBatchEntryDTO : BindingModelBase<LoanDisbursementBatchEntryDTO>
    {
        public LoanDisbursementBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Loan Disbursement Batch")]
        [ValidGuid]
        public Guid LoanDisbursementBatchId { get; set; }

        [DataMember]
        [Display(Name = "Loan Disbursement Batch Number")]
        public int LoanDisbursementBatchBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Loan Disbursement Batch Reference")]
        public string LoanDisbursementBatchReference { get; set; }

        [DataMember]
        [Display(Name = "Loan Disbursement Batch Priority")]
        public int LoanDisbursementBatchPriority { get; set; }

        [DataMember]
        [Display(Name = "Loan Case")]
        [ValidGuid]
        public Guid LoanCaseId { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string LoanCaseReference { get; set; }

        [DataMember]
        [Display(Name = "Loan Case Parent")]
        public Guid? LoanCaseParentId { get; set; }

        [DataMember]
        [Display(Name = "Loan Case Branch")]
        public Guid LoanCaseBranchId { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public int LoanCaseCaseNumber { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public string LoanCasePaddedCaseNumber
        {
            get
            {
                return LoanCaseId != Guid.Empty ? string.Format("{0}", LoanCaseCaseNumber).PadLeft(7, '0') : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Is Batched?")]
        public bool LoanCaseIsBatched { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int LoanCaseBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batched By")]
        public string LoanCaseBatchedBy { get; set; }

        [DataMember]
        [Display(Name = "Loan Status")]
        public int LoanCaseStatus { get; set; }

        [DataMember]
        [Display(Name = "Loan Status")]
        public string LoanCaseStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanCaseStatus), LoanCaseStatus) ? EnumHelper.GetDescription((LoanCaseStatus)LoanCaseStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Approved Standing Order Principal")]
        public decimal LoanCaseApprovedPrincipalPayment { get; set; }

        [DataMember]
        [Display(Name = "Approved Standing Order Interest")]
        public decimal LoanCaseApprovedInterestPayment { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public Guid LoanCaseLoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product Code")]
        public int LoanCaseLoanProductCode { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public string LoanCaseLoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Loan Product Principal G/L Account")]
        public Guid LoanCaseLoanProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product Interest Received G/L Account")]
        public Guid LoanCaseLoanProductInterestReceivedChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product Interest Receivable G/L Account")]
        public Guid LoanCaseLoanProductInterestReceivableChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product Interest Charged G/L Account")]
        public Guid LoanCaseLoanProductInterestChargedChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Minimum Interest Chargeable Amount")]
        public decimal LoanCaseLoanRegistrationMinimumInterestAmount { get; set; }

        [DataMember]
        [Display(Name = "Payment Frequency Per Year")]
        public int LoanCaseLoanRegistrationPaymentFrequencyPerYear { get; set; }

        [DataMember]
        [Display(Name = "Microcredit?")]
        public bool LoanCaseLoanRegistrationMicrocredit { get; set; }

        [DataMember]
        [Display(Name = "Interest Charge Mode")]
        public int LoanCaseLoanInterestChargeMode { get; set; }

        [DataMember]
        [Display(Name = "Interest Recovery Mode")]
        public int LoanCaseLoanInterestRecoveryMode { get; set; }

        [DataMember]
        [Display(Name = "Loan Product Loan Balance")]
        public decimal LoanCaseLoanProductLoanBalance { get; set; }

        [DataMember]
        [Display(Name = "Loan Purpose")]
        public string LoanCaseLoanPurposeDescription { get; set; }

        [DataMember]
        [Display(Name = "Term (Months)")]
        public int LoanCaseLoanRegistrationTermInMonths { get; set; }
        
        [DataMember]
        [Display(Name = "Standing Order Trigger")]
        public int LoanCaseLoanRegistrationStandingOrderTrigger { get; set; }

        [DataMember]
        [Display(Name = "Grace Period")]
        public int LoanCaseLoanRegistrationGracePeriod { get; set; }

        [DataMember]
        [Display(Name = "Payment Due Date")]
        public int LoanCaseLoanRegistrationPaymentDueDate { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public int LoanCaseLoanRegistrationLoanProductCategory { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string LoanCaseLoanRegistrationLoanProductCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductCategory), LoanCaseLoanRegistrationLoanProductCategory) ? EnumHelper.GetDescription((LoanProductCategory)LoanCaseLoanRegistrationLoanProductCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public int LoanCaseLoanRegistrationRoundingType { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public string LoanCaseLoanRegistrationRoundingTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RoundingType), LoanCaseLoanRegistrationRoundingType) ? EnumHelper.GetDescription((RoundingType)LoanCaseLoanRegistrationRoundingType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Interest Calculation Mode")]
        public int LoanCaseLoanInterestCalculationMode { get; set; }

        [DataMember]
        [Display(Name = "Annual Percentage Rate")]
        public double LoanCaseLoanInterestAnnualPercentageRate { get; set; }

        [DataMember]
        [Display(Name = "Approved Amount")]
        public decimal LoanCaseApprovedAmount { get; set; }

        [DataMember]
        [Display(Name = "Audit Top-Up Amount")]
        public decimal LoanCaseAuditTopUpAmount { get; set; }

        [DataMember]
        [Display(Name = "Monthly Payback Amount")]
        public decimal LoanCaseMonthlyPaybackAmount { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid LoanCaseCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int LoanCaseCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string LoanCaseCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), LoanCaseCustomerType) ? EnumHelper.GetDescription((CustomerType)LoanCaseCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int LoanCaseCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string LoanCaseCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), LoanCaseCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)LoanCaseCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int LoanCaseCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedLoanCaseCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", LoanCaseCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string LoanCaseCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string LoanCaseCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string LoanCaseCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string LoanCaseCustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string LoanCaseCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? LoanCaseCustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)LoanCaseCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", LoanCaseCustomerIndividualSalutationDescription, LoanCaseCustomerIndividualFirstName, LoanCaseCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = LoanCaseCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string LoanCaseCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string LoanCaseCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string LoanCaseCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string LoanCaseCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string LoanCaseCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string LoanCaseCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string LoanCaseCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Received Date")]
        public DateTime LoanCaseReceivedDate { get; set; }

        [DataMember]
        [Display(Name = "Loan Created Date")]
        public DateTime LoanCaseCreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Savings Product")]
        public Guid LoanCaseSavingsProductId { get; set; }

        [DataMember]
        [Display(Name = "Savings Product Code")]
        public int LoanCaseSavingsProductCode { get; set; }

        [Display(Name = "Savings Product")]
        public string LoanCaseSavingsProductDescription { get; set; }

        [Display(Name = "Savings Product G/L Account")]
        public Guid LoanCaseSavingsProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchEntryStatus), Status) ? EnumHelper.GetDescription((BatchEntryStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
