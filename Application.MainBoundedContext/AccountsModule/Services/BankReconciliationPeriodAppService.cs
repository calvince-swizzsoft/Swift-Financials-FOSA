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

        public BankReconciliationPeriodAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<BankReconciliationPeriod> bankReconciliationPeriodRepository,
           IRepository<BankReconciliationEntry> bankReconciliationEntryRepository,
           IJournalEntryPostingService journalEntryPostingService,
           ISqlCommandAppService sqlCommandAppService)
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
        }

        public BankReconciliationPeriodDTO AddNewBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader)
        {
            if (bankReconciliationPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(bankReconciliationPeriodDTO.DurationStartDate, bankReconciliationPeriodDTO.DurationEndDate);

                    var bankReconciliationPeriod = BankReconciliationPeriodFactory.CreateBankReconciliationPeriod(bankReconciliationPeriodDTO.BranchId, bankReconciliationPeriodDTO.PostingPeriodId, bankReconciliationPeriodDTO.BankLinkageId, bankReconciliationPeriodDTO.ChartOfAccountId, bankReconciliationPeriodDTO.BankAccountNumber, duration, bankReconciliationPeriodDTO.BankAccountBalance, bankReconciliationPeriodDTO.GeneralLedgerAccountBalance, bankReconciliationPeriodDTO.Remarks);

                    bankReconciliationPeriod.Status = (int)BankReconciliationPeriodStatus.Open;
                    bankReconciliationPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                    _bankReconciliationPeriodRepository.Add(bankReconciliationPeriod, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return bankReconciliationPeriod.ProjectedAs<BankReconciliationPeriodDTO>();
                }
            }
            else return null;
        }

        public bool UpdateBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader)
        {
            if (bankReconciliationPeriodDTO == null || bankReconciliationPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _bankReconciliationPeriodRepository.Get(bankReconciliationPeriodDTO.Id, serviceHeader);

                if (persisted != null)
                {
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

        public bool CloseBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, int bankReconciliationPeriodAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (bankReconciliationPeriodDTO != null)
            {
                var journals = new List<Journal>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _bankReconciliationPeriodRepository.Get(bankReconciliationPeriodDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)BankReconciliationPeriodStatus.Open)
                    {
                        switch ((BankReconciliationPeriodAuthOption)bankReconciliationPeriodAuthOption)
                        {
                            case BankReconciliationPeriodAuthOption.Post:

                                var bankReconciliationEntries = FindBankReconciliationEntriesByBankReconciliationPeriodId(persisted.Id, serviceHeader);

                                if (bankReconciliationEntries != null && bankReconciliationEntries.Any())
                                {
                                    int counter = 0;

                                    foreach (var bankReconciliationEntryDTO in bankReconciliationEntries)
                                    {
                                        counter += 1;

                                        var secondaryDescription = string.Format("{0}~{1},{2}", bankReconciliationPeriodDTO.BankAccountNumber, bankReconciliationPeriodDTO.BankLinkageBankName, bankReconciliationPeriodDTO.BankLinkageBankBranchName);

                                        var reference = string.Format("{0}~({1}/{2})", bankReconciliationPeriodDTO.Remarks, counter, bankReconciliationEntries.Count);

                                        switch ((BankReconciliationAdjustmentType)bankReconciliationEntryDTO.AdjustmentType)
                                        {
                                            case BankReconciliationAdjustmentType.BankAccountDebit:

                                                if (bankReconciliationEntryDTO.ChartOfAccountId != null && bankReconciliationEntryDTO.ChartOfAccountId != Guid.Empty)
                                                {
                                                    var bankAccountDebitJournal = JournalFactory.CreateJournal(null, bankReconciliationPeriodDTO.PostingPeriodId, bankReconciliationPeriodDTO.BranchId, null, bankReconciliationEntryDTO.Value, bankReconciliationEntryDTO.Remarks, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.BankReconciliation, UberUtil.GetLastDayOfMonth(persisted.Duration.EndDate), serviceHeader);
                                                    _journalEntryPostingService.PerformDoubleEntry(bankAccountDebitJournal, bankReconciliationPeriodDTO.ChartOfAccountId, bankReconciliationEntryDTO.ChartOfAccountId.Value, serviceHeader);
                                                    journals.Add(bankAccountDebitJournal);
                                                }

                                                break;
                                            case BankReconciliationAdjustmentType.BankAccountCredit:

                                                if (bankReconciliationEntryDTO.ChartOfAccountId != null && bankReconciliationEntryDTO.ChartOfAccountId != Guid.Empty)
                                                {
                                                    var bankAccountDebitCredit = JournalFactory.CreateJournal(null, bankReconciliationPeriodDTO.PostingPeriodId, bankReconciliationPeriodDTO.BranchId, null, bankReconciliationEntryDTO.Value, bankReconciliationEntryDTO.Remarks, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.BankReconciliation, UberUtil.GetLastDayOfMonth(persisted.Duration.EndDate), serviceHeader);
                                                    _journalEntryPostingService.PerformDoubleEntry(bankAccountDebitCredit, bankReconciliationEntryDTO.ChartOfAccountId.Value, bankReconciliationPeriodDTO.ChartOfAccountId, serviceHeader);
                                                    journals.Add(bankAccountDebitCredit);
                                                }

                                                break;
                                            case BankReconciliationAdjustmentType.GeneralLedgerAccountDebit:
                                            case BankReconciliationAdjustmentType.GeneralLedgerAccountCredit:
                                            default:
                                                break;
                                        }
                                    }
                                }

                                persisted.Status = (int)BankReconciliationPeriodStatus.Closed;
                                persisted.AuthorizationRemarks = bankReconciliationPeriodDTO.AuthorizationRemarks;
                                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                                persisted.AuthorizedDate = DateTime.Now;

                                break;
                            case BankReconciliationPeriodAuthOption.Reject:

                                persisted.Status = (int)BankReconciliationPeriodStatus.Suspended;
                                persisted.AuthorizationRemarks = bankReconciliationPeriodDTO.AuthorizationRemarks;
                                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                                persisted.AuthorizedDate = DateTime.Now;

                                break;
                            default:
                                break;
                        }

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }

                if (result && journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
            }

            return result;
        }

        public BankReconciliationEntryDTO AddNewBankReconciliationEntry(BankReconciliationEntryDTO bankReconciliationEntryDTO, ServiceHeader serviceHeader)
        {
            if (bankReconciliationEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
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

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in bankReconciliationEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _bankReconciliationEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
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
                        return bankReconciliationPeriod.ProjectedAs<BankReconciliationPeriodDTO>();
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
