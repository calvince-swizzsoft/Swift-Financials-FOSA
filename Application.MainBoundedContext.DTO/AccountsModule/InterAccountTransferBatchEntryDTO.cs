using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
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
    public class InterAccountTransferBatchEntryDTO : BindingModelBase<InterAccountTransferBatchEntryDTO>
    {
        public InterAccountTransferBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Inter-Account Transfer Batch")]
        [ValidGuid]
        public Guid InterAccountTransferBatchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid InterAccountTransferBatchBranchId { get; set; }

        [Display(Name = "Branch")]
        public string InterAccountTransferBatchBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int InterAccountTransferBatchBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedInterAccountTransferBatchBatchNumber
        {
            get
            {
                return string.Format("{0}", InterAccountTransferBatchBatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Batch Reference")]
        public string InterAccountTransferBatchReference { get; set; }

        [Display(Name = "Apportion To")]
        public int ApportionTo { get; set; }

        [Display(Name = "Apportion To")]
        public string ApportionToDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ApportionTo), ApportionTo) ? EnumHelper.GetDescription((ApportionTo)ApportionTo) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
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
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid? CustomerAccountId { get; set; }

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
                if (CustomerAccountId != null)
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
        [Display(Name = "Principal")]
        public decimal Principal { get; set; }

        [DataMember]
        [Display(Name = "Interest")]
        public decimal Interest { get; set; }

        [DataMember]
        [Display(Name = "Primary Description")]
        [Required]
        public string PrimaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Secondary Description")]
        [Required]
        public string SecondaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription { get; set; }
        

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }




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
        [Display(Name = "Book Balance")]
        public decimal BookBalance { get; set; }


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
        [Display(Name = "Personal Id Number")]
        public string CustomerPersonalIdentificationNumber { get; set; }
    }
}
