using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalVoucherDTO : BindingModelBase<JournalVoucherDTO>
    {
        public JournalVoucherDTO()
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
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public short ChartOfAccountAccountType { get; set; }

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
                return string.Format("{0}-{1} {2}", ChartOfAccountAccountType, ChartOfAccountAccountCode, ChartOfAccountAccountName);
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

        public Guid? CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public short CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product")]
        public Guid CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public byte CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public short CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

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
        public byte CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), (int)CustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public byte CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), (int)CustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation) : string.Empty;
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
        [Display(Name = "Customer")]
        public Guid CustomerAccountCustomerId { get; set; }

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
        [Display(Name = "Voucher Number")]
        public int VoucherNumber { get; set; }

        [DataMember]
        [Display(Name = "Voucher Number")]
        public string PaddedVoucherNumber
        {
            get
            {
                return string.Format("{0}", VoucherNumber).PadLeft(6, '0');
            }
        }

        [DataMember]
        [Display(Name = "Principal Amount")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Primary Description")]
        public string PrimaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Secondary Description")]
        public string SecondaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Value Date")]
        public DateTime? ValueDate { get; set; }

        [DataMember]
        [Display(Name = "Journal Voucher Status")]
        public byte Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(JournalVoucherStatus), (int)Status) ? EnumHelper.GetDescription((JournalVoucherStatus)Status) : string.Empty;
            }
        }


        [DataMember]
        [Display(Name = "Journal Voucher Auth Option")]
        public byte AuthOption { get; set; }

        [DataMember]
        [Display(Name = "Journal Voucher Auth Option")]
        public string AuthOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(JournalVoucherAuthOption), (int)AuthOption) ? EnumHelper.GetDescription((JournalVoucherAuthOption)AuthOption) : string.Empty;
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

        [DataMember]
        [Display(Name = "Can Suppress Maker/Checker Validation?")]
        public bool CanSuppressMakerCheckerValidation { get; set; }







        public ObservableCollection<JournalVoucherEntryDTO> JournalVoucherEntries { get; set; }

        public JournalVoucherEntryDTO JournalVoucherEntry { get; set; }

        [DataMember]
        public string ErrorMessageResult { get; set; }


        [DataMember]
        public int rowId { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }


        [DataMember]
        [Display(Name = "Payment Method")]
        public DateTime PaymentMethod { get; set; }

    }
}
