using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class InterAccountTransferBatchDTO : BindingModelBase<InterAccountTransferBatchDTO>
    {
        public InterAccountTransferBatchDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product")]
        public Guid CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string CustomerAccountCustomerAccountTypeProductCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ProductCode), CustomerAccountCustomerAccountTypeProductCode) ? EnumHelper.GetDescription((ProductCode)CustomerAccountCustomerAccountTypeProductCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullAccountNumber
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
        [Display(Name = "Customer Salutation")]
        public int CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerAccountCustomerFullName
        {
            get
            {
                if (CustomerAccountId != null)
                {
                    return string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName);
                }
                else return string.Empty;
            }
        }

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
        [Display(Name = "Batch Number")]
        public int BatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedBatchNumber
        {
            get
            {
                return string.Format("{0}", BatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Reference")]
        //[Required]
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
                return Enum.IsDefined(typeof(BatchStatus), Status) ? EnumHelper.GetDescription((BatchStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Verified/Rejected By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verification/Rejection Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected Date")]
        public DateTime? AuditedDate { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected Date")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Posted Entries")]
        public string PostedEntries { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public CustomerAccountDTO Customers { get; set; }
        public CustomerAccountDTO interAccountTransferBatch { get; set; }


        //Additional DTOS

        [DataMember]
        [Display(Name = "Account Status")]
        public int AccountStatus { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string AccountStatusDescription { get; set; }
        

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Available Balance")]
        public decimal AvailableBalance { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public String AccountNumber { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public byte CustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription { get; set; }
       
        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName { get; set; }

        [DataMember]
        [Display(Name = "Customer Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string CustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        public ObservableCollection<InterAccountTransferBatchEntryDTO> interAccountBatchEntries { get; set; }

        
        [DataMember]
        [Display(Name = "Wire Transfer Auth Option")]
        public byte WireTransferAuthOption { get; set; }

        [DataMember]
        [Display(Name = "WireTransfer Batch AuthOption")]
        public string WireTransferAuthOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchAuthOption), (int)WireTransferAuthOption) ? EnumHelper.GetDescription((BatchAuthOption)WireTransferAuthOption) : string.Empty;
            }
        }
        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

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
    }
}
