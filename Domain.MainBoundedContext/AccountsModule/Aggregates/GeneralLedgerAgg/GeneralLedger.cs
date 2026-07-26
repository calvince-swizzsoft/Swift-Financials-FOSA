using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerAgg
{
    public class GeneralLedger : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public int LedgerNumber { get; set; }

        public decimal TotalValue { get; set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }
        
        HashSet<GeneralLedgerEntry> _generalLedgerEntries;
        public virtual ICollection<GeneralLedgerEntry> GeneralLedgerEntries
        {
            get
            {
                if (_generalLedgerEntries == null)
                {
                    _generalLedgerEntries = new HashSet<GeneralLedgerEntry>();
                }
                return _generalLedgerEntries;
            }
            private set
            {
                _generalLedgerEntries = new HashSet<GeneralLedgerEntry>(value);
            }
        }
    }
}
