using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class GeneralLedgerEntryDTO : BindingModelBase<GeneralLedgerEntryDTO>
    {
        public GeneralLedgerEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "General Ledger")]
        [ValidGuid]
        public Guid GeneralLedgerId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account")]
       [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account Name")]
        public string ChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Credit G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Credit G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account")]
        [ValidGuid]
        public Guid ContraChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit Debit G/L Account Type")]
        public int ContraChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Code")]
        public int ContraChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Name")]
        public string ContraChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Name")]
        public string ContraChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ContraChartOfAccountAccountType.FirstDigit(), ContraChartOfAccountAccountCode, ContraChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Debit G/L Account Cost Center")]
        public Guid? ContraChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Cost Center")]
        public string ContraChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account")]
        public Guid? CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account BranchCode")]
        public int? CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account ProductCode")]
        public int? CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account TargetProductId")]
        public Guid? CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Account TargetProductCode")]
        public int? CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [Display(Name = "Credit Customer Account TargetProductDescription")]
        public string CustomerAccountAccountTypeTargetProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public byte EntryType { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string EntryTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(GeneralLedgerEntryType), (int)EntryType) ? EnumHelper.GetDescription((GeneralLedgerEntryType)EntryType) : string.Empty;
            }
        }
        [DataMember]
        [Display(Name = "Credit Full Account Number")]
        public string CustomerAccountFullAccountNumber
        {
            get
            {
                if (CustomerAccountId.HasValue)
                {
                    var branchCode = string.Format("{0}", CustomerAccountBranchCode.HasValue ? CustomerAccountBranchCode : 0);

                    var membershipNumber = string.Format("{0}", CustomerAccountCustomerSerialNumber.HasValue ? CustomerAccountCustomerSerialNumber : 0);

                    var productCode = string.Format("{0}", CustomerAccountCustomerAccountTypeProductCode.HasValue ? CustomerAccountCustomerAccountTypeProductCode : 0);

                    var targetProductCode = string.Format("{0}", CustomerAccountCustomerAccountTypeTargetProductCode.HasValue ? CustomerAccountCustomerAccountTypeTargetProductCode : 0);

                    return string.Format("{0}-{1}-{2}-{3}",
                                    branchCode.PadLeft(3, '0'),
                                    membershipNumber.PadLeft(6, '0'),
                                    productCode.PadLeft(3, '0'),
                                    targetProductCode.PadLeft(3, '0'));
                }
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Credit Customer")]
        public Guid? CustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Type")]
        public int CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Salutation")]
        public int? CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return CustomerAccountCustomerIndividualSalutation.HasValue && Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation.Value) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation.Value) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Credit Customer First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Credit Serial Number")]
        public int? CustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Credit Serial Number")]
        public string PaddedCustomerAccountCustomerSerialNumber
        {
            get
            {
                return CustomerAccountCustomerId.HasValue ? string.Format("{0}", CustomerAccountCustomerSerialNumber).PadLeft(7, '0') : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Credit Payroll Numbers")]
        public string CustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Credit Identity Card Number")]
        public string CustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Credit Group Name")]
        public string CustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Credit Customer Name")]
        public string CustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                if (CustomerAccountId.HasValue)
                {
                    switch ((CustomerType)CustomerAccountCustomerType)
                    {
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                            result = string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName).Trim();
                            break;
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
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
        [Display(Name = "Credit Account Number")]
        public string CustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Credit Membership Number")]
        public string CustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Credit Personal File Number")]
        public string CustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account")]
        public Guid? ContraCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account BranchCode")]
        public int? ContraCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account ProductCode")]
        public int? ContraCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account TargetProductId")]
        public Guid? ContraCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account TargetProductCode")]
        public int? ContraCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [Display(Name = "Debit Customer Account TargetProductDescription")]
        public string ContraCustomerAccountAccountTypeTargetProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Debit Full Account Number")]
        public string ContraCustomerAccountFullAccountNumber
        {
            get
            {
                if (ContraCustomerAccountId.HasValue)
                {
                    var branchCode = string.Format("{0}", ContraCustomerAccountBranchCode.HasValue ? ContraCustomerAccountBranchCode : 0);

                    var membershipNumber = string.Format("{0}", ContraCustomerAccountCustomerSerialNumber.HasValue ? ContraCustomerAccountCustomerSerialNumber : 0);

                    var productCode = string.Format("{0}", ContraCustomerAccountCustomerAccountTypeProductCode.HasValue ? ContraCustomerAccountCustomerAccountTypeProductCode : 0);

                    var targetProductCode = string.Format("{0}", ContraCustomerAccountCustomerAccountTypeTargetProductCode.HasValue ? ContraCustomerAccountCustomerAccountTypeTargetProductCode : 0);

                    return string.Format("{0}-{1}-{2}-{3}",
                                    branchCode.PadLeft(3, '0'),
                                    membershipNumber.PadLeft(6, '0'),
                                    productCode.PadLeft(3, '0'),
                                    targetProductCode.PadLeft(3, '0'));
                }
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Debit Customer")]
        public Guid? ContraCustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Type")]
        public int ContraCustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Salutation")]
        public int? ContraCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Salutation")]
        public string ContraCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return ContraCustomerAccountCustomerIndividualSalutation.HasValue && Enum.IsDefined(typeof(Salutation), ContraCustomerAccountCustomerIndividualSalutation.Value) ? EnumHelper.GetDescription((Salutation)ContraCustomerAccountCustomerIndividualSalutation.Value) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Debit Customer First Name")]
        public string ContraCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Other Names")]
        public string ContraCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Debit Serial Number")]
        public int? ContraCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Debit Serial Number")]
        public string PaddedContraCustomerAccountCustomerSerialNumber
        {
            get
            {
                return ContraCustomerAccountCustomerId.HasValue ? string.Format("{0}", ContraCustomerAccountCustomerSerialNumber).PadLeft(7, '0') : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Debit Payroll Numbers")]
        public string ContraCustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Debit Identity Card Number")]
        public string ContraCustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Debit Group Name")]
        public string ContraCustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Name")]
        public string ContraCustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                if (ContraCustomerAccountId.HasValue)
                {
                    switch ((CustomerType)ContraCustomerAccountCustomerType)
                    {
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                            result = string.Format("{0} {1} {2}", ContraCustomerAccountCustomerIndividualSalutationDescription, ContraCustomerAccountCustomerIndividualFirstName, ContraCustomerAccountCustomerIndividualLastName).Trim();
                            break;
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                        case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                            result = ContraCustomerAccountCustomerNonIndividualDescription;
                            break;
                        default:
                            break;
                    }
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Debit Account Number")]
        public string ContraCustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Debit Membership Number")]
        public string ContraCustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Debit Personal File Number")]
        public string ContraCustomerAccountCustomerReference3 { get; set; }

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
        [Display(Name = "Amount")]
        // Allow negative numbers for credit entries
        [RegularExpression(@"^\s*-?\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be a valid number with up to 2 decimal places!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Value Date")]
        public DateTime? ValueDate { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(GeneralLedgerEntryStatus), Status) ? EnumHelper.GetDescription((GeneralLedgerEntryStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }


        [DataMember]
        [Display(Name = "Credit Account Number")]
        public string CreditFullAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "Debit Account Number")]
        public string DebitFullAccountNumber { get; set; }
        public object ErrorMessageResult { get; set; }
    }
}
