
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
    public class OverDeductionBatchEntryDTO : BindingModelBase<OverDeductionBatchEntryDTO>
    {
        public OverDeductionBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Refund Batch")]
        [ValidGuid]
        public Guid OverDeductionBatchId { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account")]
        [ValidGuid]
        public Guid DebitCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Branch Id")]
        public Guid DebitCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Branch Code")]
        public int DebitCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account Product")]
        public Guid DebitCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account Product Code")]
        public int DebitCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account Target Product Code")]
        public int DebitCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Serial Number")]
        public int DebitCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account Number")]
        public string DebitFullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            DebitCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            DebitCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            DebitCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            DebitCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Debit Customer Salutation")]
        public int DebitCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Salutation")]
        public string DebitCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), DebitCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)DebitCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Debit Customer First Name")]
        public string DebitCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Other Names")]
        public string DebitCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Name")]
        public string DebitCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", DebitCustomerAccountCustomerIndividualSalutationDescription, DebitCustomerAccountCustomerIndividualFirstName, DebitCustomerAccountCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Debit Product Name")]
        public string DebitProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Debit Product G/L Account")]
        public Guid DebitProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Debit Product G/L Account Code")]
        public int DebitProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Debit Product G/L Account Name")]
        public string DebitProductChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account")]
        [ValidGuid]
        public Guid CreditCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account Branch Id")]
        public Guid CreditCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account Branch Code")]
        public int CreditCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account Product")]
        public Guid CreditCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account Product Code")]
        public int CreditCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account Target Product Code")]
        public int CreditCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Serial Number")]
        public int CreditCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account Number")]
        public string CreditFullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            CreditCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            CreditCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CreditCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CreditCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CreditCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CreditCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CreditCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CreditCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Credit First Name")]
        public string CreditCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Credit Other Names")]
        public string CreditCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Name")]
        public string CreditCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", CreditCustomerAccountCustomerIndividualSalutationDescription, CreditCustomerAccountCustomerIndividualFirstName, CreditCustomerAccountCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Credit Product Name")]
        public string CreditProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Credit Product G/L Account")]
        public Guid CreditProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit Product G/L Account Code")]
        public int CreditProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Product G/L Account Name")]
        public string CreditProductChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Principal")]
        public decimal Principal { get; set; }

        [DataMember]
        [Display(Name = "Interest")]
        public decimal Interest { get; set; }

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


        //Additional DTOs
        [DataMember]
        public CustomerAccountDTO customer { get; set; }


        [DataMember]
        [Display(Name = "CreditAccount Full Account Number")]
        public string CreditCustomerAccountFullAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "DebitAccount Full Account Number")]
        public string DebitCustomerAccountFullAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer")]
        public string CreditCustomerAccountFullName { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer")]
        public string DebitCustomerAccountFullName { get; set; }

        [DataMember]
        [Display(Name = "Identification Number")]
        public string CreditCustomerIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Identification Number")]
        public string DebitCustomerIdentificationNumber { get; set; }

        [DataMember]
        public CustomerAccountDTO CreditCustomerAccountDTO { get; set; }

        [DataMember]
        public CustomerAccountDTO DebitCustomerAccountDTO { get; set; }


    }
}
