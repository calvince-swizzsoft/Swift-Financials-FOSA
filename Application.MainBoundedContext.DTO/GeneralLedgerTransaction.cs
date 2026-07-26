using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO
{
    public class GeneralLedgerTransaction
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [Display(Name = "G/L Account")]
        public Guid GLAccountId { get; set; }

        [Display(Name = "G/L Account Code")]
        public int GLAccountCode { get; set; }

        [Display(Name = "G/L Account Type ")]
        public int GLAccountType { get; set; }

        [Display(Name = "G/L Account Description")]
        public string GLAccountDescription { get; set; }

        [Display(Name = "G/L Account Name")]
        public string GLAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", GLAccountType.FirstDigit(), GLAccountCode, GLAccountDescription);
            }
        }

        [Display(Name = "Customer Name")]
        public string CustomerFullName { get; set; }

        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [Display(Name = "Customer Account")]
        public Guid? CustomerAccountId { get; set; }

        [Display(Name = "Product Code")]
        public int? CustomerAccountAccountTypeProductCode { get; set; }

        [Display(Name = "Target Product")]
        public Guid? CustomerAccountAccountTypeTargetProductId { get; set; }

        [Display(Name = "Target Product Code")]
        public int? CustomerAccountAccountTypeTargetProductCode { get; set; }

        [Display(Name = "Target Product Name")]
        public string CustomerAccountAccountTypeTargetProductDescription { get; set; }

        [Display(Name = "Full Account Number")]
        public string CustomerAccountNumber { get; set; }

        [Display(Name = "Contra G/L Account")]
        public Guid ContraGLAccountId { get; set; }

        [Display(Name = "Contra G/L Account Code")]
        public int ContraGLAccountCode { get; set; }

        [Display(Name = "G/L Account Type ")]
        public int ContraGLAccountType { get; set; }

        [Display(Name = "Contra G/L Account Description")]
        public string ContraGLAccountDescription { get; set; }

        [Display(Name = "Contra G/L Account Name")]
        public string ContraGLAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ContraGLAccountType.FirstDigit(), ContraGLAccountCode, ContraGLAccountDescription);
            }
        }

        [Display(Name = "Journal")]
        public Guid JournalId { get; set; }

        [Display(Name = "Journal Parent")]
        public Guid? JournalParentId { get; set; }

        [Display(Name = "Primary Description")]
        public string JournalPrimaryDescription { get; set; }

        [Display(Name = "Secondary Description")]
        public string JournalSecondaryDescription { get; set; }

        [Display(Name = "Reference")]
        public string JournalReference { get; set; }

        [Display(Name = "Debit")]
        public decimal Debit { get; set; }

        [Display(Name = "Credit")]
        public decimal Credit { get; set; }

        [Display(Name = "Book Balance")]
        public decimal BookBalance { get; set; }

        [Display(Name = "Available Balance")]
        public decimal AvailableBalance { get; set; }

        [Display(Name = "Running Balance")]
        public decimal RunningBalance { get; set; }

        [Display(Name = "Transaction Code")]
        public int JournalTransactionCode { get; set; }

        [Display(Name = "Transaction Code")]
        public string JournalTransactionCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemTransactionCode), JournalTransactionCode) ? EnumHelper.GetDescription((SystemTransactionCode)JournalTransactionCode) : string.Empty;
            }
        }

        [Display(Name = "Value Date")]
        public DateTime? JournalValueDate { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime JournalCreatedDate { get; set; }

        [Display(Name = "Is Locked?")] /*locked when reversed*/
        public bool JournalIsLocked { get; set; }

        [Display(Name = "App. User Name")]
        public string ApplicationUserName { get; set; }

        [Display(Name = "Env. User Name")]
        public string EnvironmentUserName { get; set; }

        [Display(Name = "Env. Machine Name")]
        public string EnvironmentMachineName { get; set; }

        [Display(Name = "Env. Domain Name")]
        public string EnvironmentDomainName { get; set; }

        [Display(Name = "Env. Operating System Version")]
        public string EnvironmentOSVersion { get; set; }

        [Display(Name = "Env. MAC Address")]
        public string EnvironmentMACAddress { get; set; }

        [Display(Name = "Env. Motherboard Serial Number")]
        public string EnvironmentMotherboardSerialNumber { get; set; }

        [Display(Name = "Env. Processor Id")]
        public string EnvironmentProcessorId { get; set; }

        [Display(Name = "Env. IP Address")]
        public string EnvironmentIPAddress { get; set; }
    }
}
