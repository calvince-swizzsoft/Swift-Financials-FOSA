using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherAgg
{
    public class JournalVoucher : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public byte Type { get; set; }
        
        public int VoucherNumber { get; set; }

        public decimal TotalValue { get; set; }

        public string PrimaryDescription { get; set; }

        public string SecondaryDescription { get; set; }

        public string Reference { get; set; }

        public DateTime? ValueDate { get; set; }

        public byte Status { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }
        
        HashSet<JournalVoucherEntry> _journalVoucherEntries;
        public virtual ICollection<JournalVoucherEntry> JournalVoucherEntries
        {
            get
            {
                if (_journalVoucherEntries == null)
                {
                    _journalVoucherEntries = new HashSet<JournalVoucherEntry>();
                }
                return _journalVoucherEntries;
            }
            private set
            {
                _journalVoucherEntries = new HashSet<JournalVoucherEntry>(value);
            }
        }
    }
}
