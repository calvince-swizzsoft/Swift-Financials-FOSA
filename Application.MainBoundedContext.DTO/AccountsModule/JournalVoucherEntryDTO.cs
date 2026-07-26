using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalVoucherEntryDTO : BindingModelBase<JournalVoucherEntryDTO>
    {
        public JournalVoucherEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Journal Voucher")]
        [ValidGuid]
        public Guid JournalVoucherId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }


        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

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
        [Display(Name = "Type")]
        public byte Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(JournalVoucherType), (int)Type) ? EnumHelper.GetDescription((JournalVoucherType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Type")]
        public byte EntryType { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string EntryTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(JournalVoucherEntryType), (int)EntryType) ? EnumHelper.GetDescription((JournalVoucherEntryType)EntryType) : string.Empty;
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
        [Display(Name = "Group Name")]
        public string CustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                if (CustomerAccountId != null)
                {
                    switch ((CustomerType)CustomerAccountCustomerType)
                    {
                        case CustomerType.Individual:
                            result = string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName).Trim();
                            break;
                        case CustomerType.Partnership:
                        case CustomerType.Corporation:
                        case CustomerType.MicroCredit:
                            result = CustomerAccountCustomerNonIndividualDescription;
                            break;
                        default:
                            break;
                    }
                }

                return result;
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
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(JournalVoucherEntryStatus), Status) ? EnumHelper.GetDescription((JournalVoucherEntryStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Primary Description")]
        public string PrimaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Secondary Description")]
        public string SecondaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Principal")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        // Additional DTOs
        [DataMember]
        public int rowId { get; set; }
    }
}
