using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalEntrySummaryDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Journal")]
        public Guid JournalId { get; set; }

        [Display(Name = "Journal Parent")]
        public Guid? JournalParentId { get; set; }

        [Display(Name = "Branch")]
        public string JournalBranchDescription { get; set; }

        [Display(Name = "Primary Description")]
        public string JournalPrimaryDescription { get; set; }

        [Display(Name = "Secondary Description")]
        public string JournalSecondaryDescription { get; set; }

        [Display(Name = "Reference")]
        public string JournalReference { get; set; }

        [Display(Name = "Transaction Code")]
        public short JournalTransactionCode { get; set; }

        [Display(Name = "Is Locked?")] /*locked when reversed*/
        public bool JournalIsLocked { get; set; }

        [Display(Name = "App. User Name")]
        public string JournalApplicationUserName { get; set; }

        [Display(Name = "Env. User Name")]
        public string JournalEnvironmentUserName { get; set; }

        [Display(Name = "Env. Machine Name")]
        public string JournalEnvironmentMachineName { get; set; }

        [Display(Name = "Env. Domain Name")]
        public string JournalEnvironmentDomainName { get; set; }

        [Display(Name = "Env. Operating System Version")]
        public string JournalEnvironmentOSVersion { get; set; }

        [Display(Name = "Env. MAC Address")]
        public string JournalEnvironmentMACAddress { get; set; }

        [Display(Name = "Env. Motherboard Serial Number")]
        public string JournalEnvironmentMotherboardSerialNumber { get; set; }

        [Display(Name = "Env. Processor Id")]
        public string JournalEnvironmentProcessorId { get; set; }

        [Display(Name = "Env. IP Address")]
        public string JournalEnvironmentIPAddress { get; set; }

        [Display(Name = "G/L Account")]
        public Guid ChartOfAccountId { get; set; }

        [Display(Name = "Contra G/L Account")]
        public Guid ContraChartOfAccountId { get; set; }

        [Display(Name = "Customer Account")]
        public Guid? CustomerAccountId { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Value Date")]
        public DateTime? ValueDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
