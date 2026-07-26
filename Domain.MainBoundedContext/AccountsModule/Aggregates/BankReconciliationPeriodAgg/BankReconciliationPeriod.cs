using Domain.MainBoundedContext.AccountsModule.Aggregates.BankLinkageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationPeriodAgg
{
    public class BankReconciliationPeriod : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public Guid BankLinkageId { get; set; }

        public virtual BankLinkage BankLinkage { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public string BankAccountNumber { get; set; }

        public virtual Duration Duration { get; set; }

        public decimal BankAccountBalance { get; set; }

        public decimal GeneralLedgerAccountBalance { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }
        
        HashSet<BankReconciliationEntry> _bankReconciliationEntries;
        public virtual ICollection<BankReconciliationEntry> BankReconciliationEntries
        {
            get
            {
                if (_bankReconciliationEntries == null)
                {
                    _bankReconciliationEntries = new HashSet<BankReconciliationEntry>();
                }
                return _bankReconciliationEntries;
            }
            private set
            {
                _bankReconciliationEntries = new HashSet<BankReconciliationEntry>(value);
            }
        }
    }
}
