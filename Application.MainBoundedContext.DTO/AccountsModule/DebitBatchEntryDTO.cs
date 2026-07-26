using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class DebitBatchEntryDTO : BindingModelBase<DebitBatchEntryDTO>
    {
        public DebitBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Batch")]
        [ValidGuid]
        public Guid DebitBatchId { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int DebitBatchBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedDebitBatchBatchNumber
        {
            get
            {
                return string.Format("{0}", DebitBatchBatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public int DebitBatchPriority { get; set; }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public string DebitBatchPriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), DebitBatchPriority) ? EnumHelper.GetDescription((QueuePriority)DebitBatchPriority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Status")]
        public int DebitBatchStatus { get; set; }

        [DataMember]
        [Display(Name = "Batch Status")]
        public string DebitBatchStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchStatus), DebitBatchStatus) ? EnumHelper.GetDescription((BatchStatus)DebitBatchStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Reference")]
        public string DebitBatchReference { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

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
                return string.Format("{0}-{1}-{2}-{3}",
                            CustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
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
        [Display(Name = "Multiplier")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Multiplier must be greater than zero!")]
        public double Multiplier { get; set; }
        
        [DataMember]
        [Display(Name = "Basis Value")]
        public decimal BasisValue { get; set; }

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


       

        public CustomerAccountDTO Customer { get; set; }



        [DataMember]
        [Display(Name = "Debit Customer")]
        public string DebitCustomerAccountFullName { get; set; }

        [DataMember]
        [Display(Name = "DebitAccount Full Account Number")]
        public string DebitCustomerAccountFullAccountNumber { get; set; }

        [Display(Name = "Debit Customer Id")]
        public string DebitCustomerAccountIdentificationNumber { get; set; }


        [Display(Name = "Debit Customer Status")]
        public string DebitCustomerAccountStatusDescription { get; set; }


        [Display(Name = "Debit Customer Remarks")]
        public string DebitCustomerAccountRemarks { get; set; }

        [Display(Name = "Debit Customer Type")]
        public string DebitCustomerAccountTypeDescription { get; set; }

        [Display(Name = "Debit Status Description")]
        public string DebitCustomerAccountStausDescription { get; set; }

    }
}
