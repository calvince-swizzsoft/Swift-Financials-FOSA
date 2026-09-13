using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class BankReconciliationPeriodAppService : IBankReconciliationPeriodAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<BankReconciliationPeriod> _bankReconciliationPeriodRepository;
        private readonly IRepository<BankReconciliationEntry> _bankReconciliationEntryRepository;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IBankLinkageAppService _bankLinkageAppService;
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;

        public BankReconciliationPeriodAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<BankReconciliationPeriod> bankReconciliationPeriodRepository,
           IRepository<BankReconciliationEntry> bankReconciliationEntryRepository,
           IJournalEntryPostingService journalEntryPostingService,
           ISqlCommandAppService sqlCommandAppService,
           IBankLinkageAppService bankLinkageAppService, IRepository<ChartOfAccount> chartOfAccountRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (bankReconciliationPeriodRepository == null)
                throw new ArgumentNullException(nameof(bankReconciliationPeriodRepository));

            if (bankReconciliationEntryRepository == null)
                throw new ArgumentNullException(nameof(bankReconciliationEntryRepository));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _bankReconciliationPeriodRepository = bankReconciliationPeriodRepository;
            _bankReconciliationEntryRepository = bankReconciliationEntryRepository;
            _journalEntryPostingService = journalEntryPostingService;
            _sqlCommandAppService = sqlCommandAppService;
            _bankLinkageAppService = bankLinkageAppService ?? throw new ArgumentNullException(nameof(bankLinkageAppService));
            _chartOfAccountRepository = chartOfAccountRepository ?? throw new ArgumentNullException(nameof(chartOfAccountRepository));
        }

        public BankReconciliationPeriodDTO AddNewBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader)
        {
            if (bankReconciliationPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
                {
                    PreparePeriod(bankReconciliationPeriodDTO, serviceHeader);
                    var duration = new Duration(bankReconciliationPeriodDTO.DurationStartDate, bankReconciliationPeriodDTO.DurationEndDate);

                    var bankReconciliationPeriod = BankReconciliationPeriodFactory.CreateBankReconciliationPeriod(bankReconciliationPeriodDTO.BranchId, bankReconciliationPeriodDTO.PostingPeriodId, bankReconciliationPeriodDTO.BankLinkageId, bankReconciliationPeriodDTO.ChartOfAccountId, bankReconciliationPeriodDTO.BankAccountNumber, duration, bankReconciliationPeriodDTO.BankAccountBalance, bankReconciliationPeriodDTO.GeneralLedgerAccountBalance, bankReconciliationPeriodDTO.Remarks);

                    bankReconciliationPeriod.Status = (int)BankReconciliationPeriodStatus.Open;
                    bankReconciliationPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                    _bankReconciliationPeriodRepository.Add(bankReconciliationPeriod, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return WithSummary(bankReconciliationPeriod.ProjectedAs<BankReconciliationPeriodDTO>(), serviceHeader);
                }
            }
            else return null;
        }

        public bool UpdateBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader)
        {
            if (bankReconciliationPeriodDTO == null || bankReconciliationPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var persisted = _bankReconciliationPeriodRepository.Get(bankReconciliationPeriodDTO.Id, serviceHeader);

                if (persisted != null && persisted.Status == (int)BankReconciliationPeriodStatus.Open)
                {
                    PreparePeriod(bankReconciliationPeriodDTO, serviceHeader);
                    var duration = new Duration(bankReconciliationPeriodDTO.DurationStartDate, bankReconciliationPeriodDTO.DurationEndDate);

                    var current = BankReconciliationPeriodFactory.CreateBankReconciliationPeriod(bankReconciliationPeriodDTO.BranchId, bankReconciliationPeriodDTO.PostingPeriodId, bankReconciliationPeriodDTO.BankLinkageId, bankReconciliationPeriodDTO.ChartOfAccountId, bankReconciliationPeriodDTO.BankAccountNumber, duration, bankReconciliationPeriodDTO.BankAccountBalance, bankReconciliationPeriodDTO.GeneralLedgerAccountBalance, bankReconciliationPeriodDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Status = persisted.Status;
                    current.CreatedBy = persisted.CreatedBy;


                    _bankReconciliationPeriodRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool CloseBankReconciliationPeriod(BankReconciliationPeriodDTO model, int authOption, int moduleNavigationItemCode, ServiceHeader header)
        {
            if (model == null || (authOption != 1 && authOption != 2)) return false;
            // Hold the period and entry-range locks until journals and status commit together.
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var period = _bankReconciliationPeriodRepository.Get(model.Id, header);
                if (period == null || period.Status != (int)BankReconciliationPeriodStatus.Open) return false;
                if (authOption == (int)BankReconciliationPeriodAuthOption.Post)
                {
                    var entries = FindBankReconciliationEntriesByBankReconciliationPeriodId(period.Id, header) ?? new List<BankReconciliationEntryDTO>();
                    foreach (var entry in entries) ValidateEntry(entry, period.ChartOfAccountId, header);
                    var ledgerBalance = ReadLedgerBalance(period.BranchId, period.ChartOfAccountId, period.Duration.EndDate, header);
                    var difference = ReconciliationDifference(period.BankAccountBalance, ledgerBalance, entries);
                    if (Math.Abs(difference) >= 0.005m)
                        throw new BankReconciliationValidationException("The current end-date bank and G/L balances do not reconcile. Refresh the period and review its adjustments.");
                    var journals = new List<Journal>();
                    foreach (var entry in entries)
                    {
                        var journal = CreateAdjustmentJournal(period, entry, moduleNavigationItemCode, header);
                        if (journal != null) journals.Add(journal);
                    }
                    // BulkSave joins this ambient scope; its nested SaveChanges cannot commit independently.
                    if (journals.Any() && !_journalEntryPostingService.BulkSave(header, journals))
                        throw new BankReconciliationValidationException("The adjustment journals could not be saved. The reconciliation remains open.");
                    period.GeneralLedgerAccountBalance = ledgerBalance;
                    period.Status = (int)BankReconciliationPeriodStatus.Closed;
                }
                else period.Status = (int)BankReconciliationPeriodStatus.Suspended;
                period.AuthorizationRemarks = model.AuthorizationRemarks;
                period.AuthorizedBy = header.ApplicationUserName;
                period.AuthorizedDate = DateTime.Now;
                return scope.SaveChanges(header) >= 0;
            }
        }

        internal static decimal ReconciliationDifference(decimal bank, decimal ledger, IEnumerable<BankReconciliationEntryDTO> entries)
        {
            foreach (var entry in entries)
            {
                switch ((BankReconciliationAdjustmentType)entry.AdjustmentType)
                {
                    case BankReconciliationAdjustmentType.BankAccountDebit: bank += entry.Value; break;
                    case BankReconciliationAdjustmentType.BankAccountCredit: bank -= entry.Value; break;
                    case BankReconciliationAdjustmentType.GeneralLedgerAccountDebit: ledger += entry.Value; break;
                    case BankReconciliationAdjustmentType.GeneralLedgerAccountCredit: ledger -= entry.Value; break;
                }
            }
            return bank - ledger;
        }

        private Journal CreateAdjustmentJournal(BankReconciliationPeriod period, BankReconciliationEntryDTO entry, int moduleCode, ServiceHeader header)
        {
            if (entry.AdjustmentType != (int)BankReconciliationAdjustmentType.GeneralLedgerAccountDebit &&
                entry.AdjustmentType != (int)BankReconciliationAdjustmentType.GeneralLedgerAccountCredit) return null;
            var journal = JournalFactory.CreateJournal(null, period.PostingPeriodId, period.BranchId, null,
                entry.Value, entry.Remarks, period.BankAccountNumber, string.Format("Bank reconciliation {0} / {1}", period.Id, entry.Id),
                moduleCode, (int)SystemTransactionCode.BankReconciliation, period.Duration.EndDate.Date, header);
            // PerformDoubleEntry takes the CREDIT account first, then the DEBIT account.
            if (entry.AdjustmentType == (int)BankReconciliationAdjustmentType.GeneralLedgerAccountDebit)
                _journalEntryPostingService.PerformDoubleEntry(journal, entry.ChartOfAccountId.Value, period.ChartOfAccountId, header);
            else _journalEntryPostingService.PerformDoubleEntry(journal, period.ChartOfAccountId, entry.ChartOfAccountId.Value, header);
            return journal;
        }

        private void ValidateEntry(BankReconciliationEntryDTO entry, Guid bankAccountId, ServiceHeader header)
        {
            if (entry.Value <= 0 || !Enum.IsDefined(typeof(BankReconciliationAdjustmentType), entry.AdjustmentType))
                throw new BankReconciliationValidationException("Choose a valid adjustment type and an amount greater than zero.");
            if (entry.AdjustmentType >= (int)BankReconciliationAdjustmentType.GeneralLedgerAccountDebit)
            {
                if (!entry.ChartOfAccountId.HasValue || entry.ChartOfAccountId == Guid.Empty || entry.ChartOfAccountId == bankAccountId)
                    throw new BankReconciliationValidationException("A G/L adjustment requires a contra account different from the bank account.");
                var account = _chartOfAccountRepository.Get(entry.ChartOfAccountId.Value, header);
                if (account == null || _chartOfAccountRepository.AllMatchingCount(new DirectSpecification<ChartOfAccount>(a => a.ParentId == entry.ChartOfAccountId.Value), header) != 0)
                    throw new BankReconciliationValidationException("Select a valid posting account as the contra G/L account.");
            }
        }

        private decimal ReadLedgerBalance(Guid branchId, Guid accountId, DateTime endDate, ServiceHeader header)
        {
            ValidateDates(endDate, endDate);
            return _bankReconciliationPeriodRepository.DatabaseSqlQuery<decimal>(@"
SELECT COALESCE(SUM(e.Amount),0) FROM dbo.swiftFin_JournalEntries e
JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
WHERE e.ChartOfAccountId=@Account AND j.BranchId=@Branch AND COALESCE(e.ValueDate,e.CreatedDate)<@End", header,
                new System.Data.SqlClient.SqlParameter("@Account", accountId), new System.Data.SqlClient.SqlParameter("@Branch", branchId),
                new System.Data.SqlClient.SqlParameter("@End", endDate.Date.AddDays(1))).Single();
        }

        public decimal FindBankReconciliationBalance(Guid bankLinkageId, DateTime endDate, ServiceHeader header)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var bank = _bankLinkageAppService.FindBankLinkage(bankLinkageId, header);
                if (bank == null || bank.ChartOfAccountId == Guid.Empty || bank.BranchId == Guid.Empty)
                    throw new BankReconciliationValidationException("Select a bank linkage with a branch and G/L account.");
                return ReadLedgerBalance(bank.BranchId, bank.ChartOfAccountId, endDate, header);
            }
        }

        private static void ValidateDates(DateTime start, DateTime end)
        {
            if (start.Year < 1753 || end.Year < 1753 || start.Date > end.Date || end.Date == DateTime.MaxValue.Date)
                throw new BankReconciliationValidationException("Choose a valid reconciliation date range.");
        }

        private void PreparePeriod(BankReconciliationPeriodDTO model, ServiceHeader header)
        {
            ValidateDates(model.DurationStartDate, model.DurationEndDate);
            var bank = _bankLinkageAppService.FindBankLinkage(model.BankLinkageId, header);
            if (bank == null || bank.ChartOfAccountId == Guid.Empty || bank.BranchId == Guid.Empty)
                throw new BankReconciliationValidationException("Select a bank linkage with a branch and G/L account.");
            model.BranchId = bank.BranchId;
            model.ChartOfAccountId = bank.ChartOfAccountId;
            model.BankAccountNumber = bank.BankAccountNumber;
            model.DurationStartDate = model.DurationStartDate.Date;
            model.DurationEndDate = model.DurationEndDate.Date;
            model.GeneralLedgerAccountBalance = ReadLedgerBalance(model.BranchId, model.ChartOfAccountId, model.DurationEndDate, header);
        }

        private BankReconciliationPeriodDTO WithSummary(BankReconciliationPeriodDTO model, ServiceHeader header)
        {
            var entries = FindBankReconciliationEntriesByBankReconciliationPeriodId(model.Id, header) ?? new List<BankReconciliationEntryDTO>();
            if (model.Status == (int)BankReconciliationPeriodStatus.Open)
                model.GeneralLedgerAccountBalance = ReadLedgerBalance(model.BranchId, model.ChartOfAccountId, model.DurationEndDate, header);
            model.EntryCount = entries.Count;
            model.BankAdjustments = entries.Sum(e => e.AdjustmentType == 0 ? e.Value : e.AdjustmentType == 1 ? -e.Value : 0);
            model.GeneralLedgerAdjustments = entries.Sum(e => e.AdjustmentType == 2 ? e.Value : e.AdjustmentType == 3 ? -e.Value : 0);
            model.AdjustedBankBalance = model.BankAccountBalance + model.BankAdjustments;
            model.AdjustedGeneralLedgerBalance = model.GeneralLedgerAccountBalance + model.GeneralLedgerAdjustments;
            model.UnreconciledBalance = model.AdjustedBankBalance - model.AdjustedGeneralLedgerBalance;
            return model;
        }


        public BankReconciliationEntryDTO AddNewBankReconciliationEntry(BankReconciliationEntryDTO bankReconciliationEntryDTO, ServiceHeader serviceHeader)
        {
            if (bankReconciliationEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var period = _bankReconciliationPeriodRepository.Get(bankReconciliationEntryDTO.BankReconciliationPeriodId, serviceHeader);
                    if (period == null || period.Status != (int)BankReconciliationPeriodStatus.Open)
                        return null;

                    ValidateEntry(bankReconciliationEntryDTO, period.ChartOfAccountId, serviceHeader);
                    if (bankReconciliationEntryDTO.AdjustmentType < 2) bankReconciliationEntryDTO.ChartOfAccountId = null;
                    var bankReconciliationEntry = BankReconciliationEntryFactory.CreateBankReconciliationEntry(bankReconciliationEntryDTO.BankReconciliationPeriodId, bankReconciliationEntryDTO.ChartOfAccountId, bankReconciliationEntryDTO.AdjustmentType, bankReconciliationEntryDTO.Value, bankReconciliationEntryDTO.ChequeNumber, bankReconciliationEntryDTO.ChequeDrawee, bankReconciliationEntryDTO.ChequeDate, bankReconciliationEntryDTO.Remarks);

                    bankReconciliationEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _bankReconciliationEntryRepository.Add(bankReconciliationEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return bankReconciliationEntry.ProjectedAs<BankReconciliationEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveBankReconciliationEntries(List<BankReconciliationEntryDTO> bankReconciliationEntryDTOs, ServiceHeader serviceHeader)
        {
            if (bankReconciliationEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                foreach (var item in bankReconciliationEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _bankReconciliationEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            if (persisted.BankReconciliationPeriodId != item.BankReconciliationPeriodId) return false;
                            var period = _bankReconciliationPeriodRepository.Get(persisted.BankReconciliationPeriodId, serviceHeader);
                            if (period == null || period.Status != (int)BankReconciliationPeriodStatus.Open)
                                return false;
                            _bankReconciliationEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<BankReconciliationPeriodDTO> FindBankReconciliationPeriods(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var bankReconciliationPeriods = _bankReconciliationPeriodRepository.GetAll(serviceHeader);

                if (bankReconciliationPeriods != null && bankReconciliationPeriods.Any())
                {
                    return bankReconciliationPeriods.ProjectedAsCollection<BankReconciliationPeriodDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BankReconciliationPeriodSpecifications.DefaultSpec();

                ISpecification<BankReconciliationPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankReconciliationPeriodPagedCollection = _bankReconciliationPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankReconciliationPeriodPagedCollection != null)
                {
                    var pageCollection = bankReconciliationPeriodPagedCollection.PageCollection.ProjectedAsCollection<BankReconciliationPeriodDTO>();

                    var itemsCount = bankReconciliationPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BankReconciliationPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BankReconciliationPeriodSpecifications.BankReconciliationPeriodFullText(text);

                ISpecification<BankReconciliationPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankReconciliationPeriodCollection = _bankReconciliationPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankReconciliationPeriodCollection != null)
                {
                    var pageCollection = bankReconciliationPeriodCollection.PageCollection.ProjectedAsCollection<BankReconciliationPeriodDTO>();

                    var itemsCount = bankReconciliationPeriodCollection.ItemsCount;

                    return new PageCollectionInfo<BankReconciliationPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public BankReconciliationPeriodDTO FindBankReconciliationPeriod(Guid bankReconciliationPeriodId, ServiceHeader serviceHeader)
        {
            if (bankReconciliationPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var bankReconciliationPeriod = _bankReconciliationPeriodRepository.Get(bankReconciliationPeriodId, serviceHeader);

                    if (bankReconciliationPeriod != null)
                    {
                        return WithSummary(bankReconciliationPeriod.ProjectedAs<BankReconciliationPeriodDTO>(), serviceHeader);
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<BankReconciliationEntryDTO> FindBankReconciliationEntriesByBankReconciliationPeriodId(Guid bankReconciliationPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (bankReconciliationPeriodId != null && bankReconciliationPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = BankReconciliationEntrySpecifications.BankReconciliationEntryFullText(bankReconciliationPeriodId, text);

                    ISpecification<BankReconciliationEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var bankReconciliationEntryPagedCollection = _bankReconciliationEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (bankReconciliationEntryPagedCollection != null)
                    {
                        var pageCollection = bankReconciliationEntryPagedCollection.PageCollection.ProjectedAsCollection<BankReconciliationEntryDTO>();

                        var itemsCount = bankReconciliationEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<BankReconciliationEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<BankReconciliationEntryDTO> FindBankReconciliationEntriesByBankReconciliationPeriodId(Guid bankReconciliationPeriodId, ServiceHeader serviceHeader)
        {
            if (bankReconciliationPeriodId != null && bankReconciliationPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = BankReconciliationEntrySpecifications.BankReconciliationEntryFullText(bankReconciliationPeriodId, null);

                    ISpecification<BankReconciliationEntry> spec = filter;

                    var bankReconciliationEntries = _bankReconciliationEntryRepository.AllMatching(spec, serviceHeader);

                    if (bankReconciliationEntries != null && bankReconciliationEntries.Any())
                    {
                        return bankReconciliationEntries.ProjectedAsCollection<BankReconciliationEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
