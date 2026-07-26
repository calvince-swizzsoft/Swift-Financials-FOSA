using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalEntryDTO : BindingModelBase<JournalEntryDTO>
    {
        public JournalEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Sequential Id")]
        public Guid SequentialId { get; set; }

        [DataMember]
        [Display(Name = "Journal")]
        public Guid JournalId { get; set; }

        [DataMember]
        [Display(Name = "Journal Parent")]
        public Guid? JournalParentId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public Guid JournalPostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string JournalPostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid JournalBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string JournalBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string JournalBranchAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string JournalBranchCompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Alternate Channel Log")]
        public Guid? JournalAlternateChannelLogId { get; set; }

        [DataMember]
        [Display(Name = "Total Value")]
        public decimal JournalTotalValue { get; set; }

        [DataMember]
        [Display(Name = "Primary Description")]
        public string JournalPrimaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Secondary Description")]
        public string JournalSecondaryDescription { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string JournalReference { get; set; }

        [DataMember]
        [Display(Name = "System Trace Audit Number")]
        public string JournalSystemTraceAuditNumber { get; set; }

        [DataMember]
        [Display(Name = "App. User Name")]
        public string JournalApplicationUserName { get; set; }

        [DataMember]
        [Display(Name = "Env. User Name")]
        public string JournalEnvironmentUserName { get; set; }

        [DataMember]
        [Display(Name = "Env. Machine Name")]
        public string JournalEnvironmentMachineName { get; set; }

        [DataMember]
        [Display(Name = "Env. Domain Name")]
        public string JournalEnvironmentDomainName { get; set; }

        [DataMember]
        [Display(Name = "Env. Operating System Version")]
        public string JournalEnvironmentOSVersion { get; set; }

        [DataMember]
        [Display(Name = "Env. MAC Address")]
        public string JournalEnvironmentMACAddress { get; set; }

        [DataMember]
        [Display(Name = "Env. Motherboard Serial Number")]
        public string JournalEnvironmentMotherboardSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Env. Processor Id")]
        public string JournalEnvironmentProcessorId { get; set; }

        [DataMember]
        [Display(Name = "Env. IP Address")]
        public string JournalEnvironmentIPAddress { get; set; }

        [DataMember]
        [Display(Name = "Module Navigation Item Code")]
        public int JournalModuleNavigationItemCode { get; set; }

        [DataMember]
        [Display(Name = "Transaction Code")]
        public int JournalTransactionCode { get; set; }

        [DataMember]
        [Display(Name = "Transaction Code")]
        public string JournalTransactionCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemTransactionCode), JournalTransactionCode) ? EnumHelper.GetDescription((SystemTransactionCode)JournalTransactionCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Value Date")]
        public DateTime? JournalValueDate { get; set; }

        [DataMember]
        [Display(Name = "Suppress Account Alert")] /*account alerts are generated when ParentId is null*/
        public bool JournalSuppressAccountAlert { get; set; }

        [DataMember]
        [Display(Name = "Transaction Date")]
        public DateTime JournalCreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")] /*locked when reversed*/
        public bool JournalIsLocked { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
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
        [Display(Name = "Contra G/L Account")]
        public Guid ContraChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Contra G/L Account Type")]
        public int ContraChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Contra G/L Account Code")]
        public int ContraChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Contra G/L Account Name")]
        public string ContraChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Contra G/L Account Name")]
        public string ContraChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ContraChartOfAccountAccountType.FirstDigit(), ContraChartOfAccountAccountCode, ContraChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Contra G/L Account Cost Center")]
        public Guid? ContraChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Contra G/L Account Cost Center")]
        public string ContraChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        public Guid? CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account BranchCode")]
        public int? CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account ProductCode")]
        public int? CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account TargetProductId")]
        public Guid? CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account TargetProductCode")]
        public int? CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [Display(Name = "Customer Account TargetProductDescription")]
        public string CustomerAccountAccountTypeTargetProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
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
        [Display(Name = "Customer")]
        public Guid? CustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public int? CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return CustomerAccountCustomerIndividualSalutation.HasValue && Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation.Value) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation.Value) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int? CustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedCustomerAccountCustomerSerialNumber
        {
            get
            {
                return CustomerAccountCustomerId.HasValue ? string.Format("{0}", CustomerAccountCustomerSerialNumber).PadLeft(7, '0') : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

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
        [Display(Name = "Value Date")]
        public DateTime? ValueDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public string IntegrityHash { get; set; }
    }
}
