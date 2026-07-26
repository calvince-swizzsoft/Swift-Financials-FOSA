using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CreditBatchEntryDTO : BindingModelBase<CreditBatchEntryDTO>
    {
        public CreditBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Batch")]
        [ValidGuid]
        public Guid CreditBatchId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        public Guid CreditBatchCreditTypeId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account")]
        public Guid CreditBatchCreditTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Code")]
        public int CreditBatchCreditTypeChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Name")]
        public string CreditBatchCreditTypeChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid CreditBatchBranchId { get; set; }

        [Display(Name = "Branch")]
        public string CreditBatchBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public Guid? CreditBatchPostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string CreditBatchPostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Posting Period Start Date")]
        public DateTime CreditBatchPostingPeriodDurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "Posting Period End Date")]
        public DateTime CreditBatchPostingPeriodDurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int CreditBatchBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedCreditBatchBatchNumber
        {
            get
            {
                return string.Format("{0}", CreditBatchBatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Batch Type")]
        public int CreditBatchType { get; set; }

        [DataMember]
        [Display(Name = "Batch Type")]
        public string CreditBatchTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CreditBatchType), CreditBatchType) ? EnumHelper.GetDescription((CreditBatchType)CreditBatchType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public int CreditBatchPriority { get; set; }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public string CreditBatchPriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), CreditBatchPriority) ? EnumHelper.GetDescription((QueuePriority)CreditBatchPriority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Status")]
        public int CreditBatchStatus { get; set; }

        [DataMember]
        [Display(Name = "Batch Status")]
        public string CreditBatchStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchStatus), CreditBatchStatus) ? EnumHelper.GetDescription((BatchStatus)CreditBatchStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Month")]
        public int CreditBatchMonth { get; set; }

        [DataMember]
        [Display(Name = "Batch Month")]
        public string CreditBatchMonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), CreditBatchMonth) ? EnumHelper.GetDescription((Month)CreditBatchMonth) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Value Date")]
        public DateTime? CreditBatchValueDate { get; set; }

        [DataMember]
        [Display(Name = "Batch Reference")]
        public string CreditBatchReference { get; set; }

        [DataMember]
        [Display(Name = "Recover Indefinite Charges?")]
        public bool CreditBatchRecoverIndefiniteCharges { get; set; }

        [DataMember]
        [Display(Name = "Recover Arrearages?")]
        public bool CreditBatchRecoverArrearages { get; set; }

        [DataMember]
        [Display(Name = "Recover Carry Forwards?")]
        public bool CreditBatchRecoverCarryForwards { get; set; }

        [DataMember]
        [Display(Name = "Enforce Month Value Date?")]
        public bool CreditBatchEnforceMonthValueDate { get; set; }
        
        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid? CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid CustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product")]
        public Guid CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string FullAccountNumber
        {
            get
            {
                if (CustomerAccountId != null && CustomerAccountId != Guid.Empty)
                {
                    return string.Format("{0}-{1}-{2}-{3}",
                                CustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                                CustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                                CustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                                CustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
                }
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAccountCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAccountCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Product Name")]
        public string ProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid ProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Code")]
        public int ProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Name")]
        public string ProductChartOfAccountName { get; set; }

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
        [Display(Name = "Principal")]
        public decimal Principal { get; set; }

        [DataMember]
        [Display(Name = "Interest")]
        public decimal Interest { get; set; }

        [DataMember]
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary")]
        public string Beneficiary { get; set; }

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
                return EnumHelper.GetDescription((BatchEntryStatus)Status);
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public CustomerAccountDTO Customer { get; set; }


        [DataMember]
        [Display(Name = "Credit Customer")]
        public string CreditCustomerAccountFullName { get; set; }

        [DataMember]
        [Display(Name = "CreditAccount Full Account Number")]
        public string CreditCustomerAccountFullAccountNumber { get; set; }

        [Display(Name = "Identification Number")]
        public string CreditCustomerAccountIdentificationNumber { get; set; }


        [Display(Name = "Credit Customer Status")]
        public string CreditCustomerAccountStatusDescription { get; set; }


        [Display(Name = "Credit Customer Remarks")]
        public string CreditCustomerAccountRemarks { get; set; }

        [Display(Name = "Credit Customer Type")]
        public string CreditCustomerAccountTypeDescription { get; set; }


        public int CreditBatchEntryFilter { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string CreditBatchEntryFilterDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CreditBatchEntryFilter), CreditBatchEntryFilter) ? EnumHelper.GetDescription((CreditBatchEntryFilter)CreditBatchEntryFilter) : string.Empty;
            }
        }


       





    }
} 

