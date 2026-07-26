using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg
{
    public class Journal : Entity
    {
        /*important for generation of tx-based account alerts*/
        public Guid? ParentId { get; set; }

        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? AlternateChannelLogId { get; set; }

        public virtual AlternateChannelLog AlternateChannelLog { get; private set; }

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

        public bool IsLocked { get; private set; }

        public string IntegrityHash { get; set; }

        HashSet<JournalEntry> _journalEntries;
        public virtual ICollection<JournalEntry> JournalEntries
        {
            get
            {
                if (_journalEntries == null)
                {
                    _journalEntries = new HashSet<JournalEntry>();
                }
                return _journalEntries;
            }
            private set
            {
                _journalEntries = new HashSet<JournalEntry>(value);
            }
        }

     

        public void PostSingleEntry(Guid chartOfAccountId, Guid? contraChartOfAccountId, decimal amount, ServiceHeader serviceHeader)
        {
            var journalEntry = JournalEntryFactory.CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, null, amount, this.ValueDate, serviceHeader);

            this.JournalEntries.Add(journalEntry);
        }

        public void PostSingleEntry(Guid chartOfAccountId, Guid contraChartOfAccountId, Guid customerAccountId, decimal amount, ServiceHeader serviceHeader)
        {
            var journalEntry = JournalEntryFactory.CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, this.ValueDate, serviceHeader);

            this.JournalEntries.Add(journalEntry);
        }

        public void PostDoubleEntries(Guid debitChartOfAccountId, Guid creditChartOfAccountId, ServiceHeader serviceHeader)
        {
            if (this.TotalValue * -1 < 0m)
            {
                #region DR

                var debitJournalEntry = JournalEntryFactory.CreateJournalEntry(this.Id, debitChartOfAccountId, creditChartOfAccountId, null, this.TotalValue, this.ValueDate, serviceHeader);

                this.JournalEntries.Add(debitJournalEntry);

                #endregion

                #region CR

                var creditJournalEntry = JournalEntryFactory.CreateJournalEntry(this.Id, creditChartOfAccountId, debitChartOfAccountId, null, this.TotalValue * -1, this.ValueDate, serviceHeader);

                this.JournalEntries.Add(creditJournalEntry);

                #endregion
            }
        }

        public void PostDoubleEntries(Guid debitChartOfAccountId, Guid creditChartOfAccountId, Guid creditCustomerAccountId, Guid debitCustomerAccountId, ServiceHeader serviceHeader)
        {
            if (this.TotalValue * -1 < 0m)
            {
                #region DR

                var debitJournalEntry = JournalEntryFactory.CreateJournalEntry(this.Id, debitChartOfAccountId, creditChartOfAccountId, debitCustomerAccountId, this.TotalValue, this.ValueDate, serviceHeader);

                this.JournalEntries.Add(debitJournalEntry);

                #endregion

                #region CR

                var creditJournalEntry = JournalEntryFactory.CreateJournalEntry(this.Id, creditChartOfAccountId, debitChartOfAccountId, creditCustomerAccountId, this.TotalValue * -1, this.ValueDate, serviceHeader);

                this.JournalEntries.Add(creditJournalEntry);

                #endregion
            }
        }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
