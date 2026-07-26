
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class WireTransferBatchEntryDTO : BindingModelBase<WireTransferBatchEntryDTO>
    {
        public WireTransferBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Batch")]
        [ValidGuid]
        public Guid WireTransferBatchId { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type")]
        public Guid WireTransferBatchWireTransferTypeId { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type G/L Account")]
        public Guid WireTransferBatchWireTransferTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type G/L Account Code")]
        public int WireTransferBatchWireTransferTypeChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type G/L Account Name")]
        public string WireTransferBatchWireTransferTypeChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid WireTransferBatchBranchId { get; set; }

        [Display(Name = "Branch")]
        public string WireTransferBatchBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int WireTransferBatchBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedWireTransferBatchBatchNumber
        {
            get
            {
                return string.Format("{0}", WireTransferBatchBatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Batch Type")]
        public int WireTransferBatchType { get; set; }

        [DataMember]
        [Display(Name = "Batch Type")]
        public string WireTransferBatchTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(WireTransferBatchType), WireTransferBatchType) ? EnumHelper.GetDescription((WireTransferBatchType)WireTransferBatchType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public int WireTransferBatchPriority { get; set; }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public string WireTransferBatchPriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), WireTransferBatchPriority) ? EnumHelper.GetDescription((QueuePriority)WireTransferBatchPriority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Status")]
        public int WireTransferBatchStatus { get; set; }

        [DataMember]
        [Display(Name = "Batch Status")]
        public string WireTransferBatchStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchStatus), WireTransferBatchStatus) ? EnumHelper.GetDescription((BatchStatus)WireTransferBatchStatus) : string.Empty;
            }
        }

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
        [Display(Name = "Customer Type")]
        public int CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer Account Product")]
        public Guid CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

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
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be greater than zero!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Payee")]
        [Required]
        public string Payee { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        [Required]
        public string AccountNumber { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Third Party Response")]
        public string ThirdPartyResponse { get; set; }

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



        [DataMember]
        [Display(Name = "WireTransferAccount Full Account Number")]
        public string WiretransferCustomerAccountFullAccountNumber { get; set; }

        

        [DataMember]
        [Display(Name = "WireTransfer Customer")]
        public string WireTranferCustomerAccountFullName { get; set; }

        [Display(Name = "Identification Number")]
        public string WireTransferAccountIdentificationNumber { get; set; }


        [Display(Name = "Account Status")]
        public string WireTransferAccountStatusDescription { get; set; }
    }
}
