using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogArchiveAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryArchiveAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FiscalCountAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalArchiveAgg
{
    public class JournalArchive : Entity
    {
        public Guid? ParentId { get; set; }

        public virtual JournalArchive Parent { get; private set; }

        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? JournalVoucherId { get; set; }

        public virtual JournalVoucher JournalVoucher { get; private set; }

        public Guid? FiscalCountId { get; set; }

        public virtual FiscalCount FiscalCount { get; private set; }

        public Guid? AlternateChannelLogArchiveId { get; set; }

        public virtual AlternateChannelLogArchive AlternateChannelLogArchive { get; private set; }

        public Guid? GeneralLedgerId { get; set; }

        public virtual GeneralLedger GeneralLedger { get; private set; }

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

        public string SystemTraceAuditNumber { get; set; }

        public int ModuleNavigationItemCode { get; set; }

        public short TransactionCode { get; set; }

        public DateTime? ValueDate { get; set; }

        public bool IsLocked { get; set; }

        

        public string IntegrityHash { get; set; }

        HashSet<JournalEntryArchive> _journalEntries;
        public virtual ICollection<JournalEntryArchive> JournalEntries
        {
            get
            {
                if (_journalEntries == null)
                {
                    _journalEntries = new HashSet<JournalEntryArchive>();
                }
                return _journalEntries;
            }
            private set
            {
                _journalEntries = new HashSet<JournalEntryArchive>(value);
            }
        }
    }
}
