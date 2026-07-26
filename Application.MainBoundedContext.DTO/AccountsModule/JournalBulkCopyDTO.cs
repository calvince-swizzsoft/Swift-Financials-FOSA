using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public Guid PostingPeriodId { get; set; }

        public Guid BranchId { get; set; }

        public Guid? AlternateChannelLogId { get; set; }

        public decimal TotalValue { get; set; }

        public string PrimaryDescription { get; set; }

        public string SecondaryDescription { get; set; }

        public string Reference { get; set; }

        public string ApplicationUserName { get; set; }

        public string EnvironmentUserName { get; set; }

        public string EnvironmentMachineName { get; set; }

        public string EnvironmentDomainName { get; set; }

        public string EnvironmentOSVersion { get; set; }

        public string EnvironmentMACAddress { get; set; }

        public string EnvironmentMotherboardSerialNumber { get; set; }

        public string EnvironmentProcessorId { get; set; }

        public string EnvironmentIPAddress { get; set; }

        public int ModuleNavigationItemCode { get; set; }

        public short TransactionCode { get; set; }

        public DateTime? ValueDate { get; set; }

        public bool SuppressAccountAlert { get; set; }

        public bool IsLocked { get; set; }

        public string IntegrityHash { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
