using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class JournalEntryAppService : IJournalEntryAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<JournalEntry> _journalEntryRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public JournalEntryAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<JournalEntry> journalEntryRepository,
            IChartOfAccountAppService chartOfAccountAppService,
            ICustomerAccountAppService customerAccountAppService,
            ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (journalEntryRepository == null)
                throw new ArgumentNullException(nameof(journalEntryRepository));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _journalEntryRepository = journalEntryRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public PageCollectionInfo<GeneralLedgerTransaction> FindLastXGeneralLedgerTransactionsByCustomerAccountId(CustomerAccountDTO customerAccountDTO, int lastXDays, int lastXItems, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            var startDate = DateTime.Today.AddDays(lastXDays * -1);

            var endDate = DateTime.Now;

            var pageCollectionInfo = FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, tallyDebitsCredits, serviceHeader);

            if (pageCollectionInfo != null)
                pageCollectionInfo.PageCollection = new List<GeneralLedgerTransaction>(pageCollectionInfo.PageCollection.TakeLast(lastXItems));

            return pageCollectionInfo;
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindLastXGeneralLedgerTransactionsByCustomerAccountIdAsync(CustomerAccountDTO customerAccountDTO, int lastXDays, int lastXItems, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            var startDate = DateTime.Today.AddDays(lastXDays * -1);

            var endDate = DateTime.Now;

            var pageCollectionInfo = await FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, tallyDebitsCredits, serviceHeader);

            if (pageCollectionInfo != null)
                pageCollectionInfo.PageCollection = new List<GeneralLedgerTransaction>(pageCollectionInfo.PageCollection.TakeLast(lastXItems));

            return pageCollectionInfo;
        }

        public PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            var transactionsList = new List<GeneralLedgerTransaction>();

            var openingAvailableBalance = 0m;

            var openingBookBalance = 0m;

            var closingAvailableBalance = 0m;

            var closingBookBalance = 0m;

            var totalCredits = 0m;

            var totalDebits = 0m;

            var itemsCount = 0;

            var pageIndex = 0;

            var pageSize = 500;

            var pageCollectionInfo = FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPage(pageIndex, pageSize, customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, tallyDebitsCredits, serviceHeader);

            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
            {
                itemsCount = pageCollectionInfo.ItemsCount;

                openingAvailableBalance = pageCollectionInfo.AvailableBalanceBroughtFoward;

                openingBookBalance = pageCollectionInfo.BookBalanceBroughtFoward;

                closingAvailableBalance = pageCollectionInfo.AvailableBalanceCarriedForward;

                closingBookBalance = pageCollectionInfo.BookBalanceCarriedForward;

                totalCredits = pageCollectionInfo.TotalCredits;

                totalDebits = pageCollectionInfo.TotalDebits;

                transactionsList.AddRange(pageCollectionInfo.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageCollectionInfo = FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPage(pageIndex, pageSize, customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, false, serviceHeader);

                        if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                        {
                            transactionsList.AddRange(pageCollectionInfo.PageCollection);

                            ++pageIndex;
                        }
                        else break;
                    }
                }
            }

            return new PageCollectionInfo<GeneralLedgerTransaction>
            {
                PageCollection = transactionsList,
                BookBalanceBroughtFoward = openingBookBalance,
                BookBalanceCarriedForward = closingBookBalance,
                AvailableBalanceBroughtFoward = openingAvailableBalance,
                AvailableBalanceCarriedForward = closingAvailableBalance,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits
            };
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            var transactionsList = new List<GeneralLedgerTransaction>();

            var openingAvailableBalance = 0m;

            var openingBookBalance = 0m;

            var closingAvailableBalance = 0m;

            var closingBookBalance = 0m;

            var totalCredits = 0m;

            var totalDebits = 0m;

            var itemsCount = 0;

            var pageIndex = 0;

            var pageSize = DefaultSettings.Instance.PageSizes[0];

            var pageItems = await FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(pageIndex, pageSize, customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, tallyDebitsCredits, serviceHeader);

            if (pageItems != null && pageItems.PageCollection != null)
            {
                itemsCount = pageItems.ItemsCount;

                openingAvailableBalance = pageItems.AvailableBalanceBroughtFoward;

                openingBookBalance = pageItems.BookBalanceBroughtFoward;

                closingAvailableBalance = pageItems.AvailableBalanceCarriedForward;

                closingBookBalance = pageItems.BookBalanceCarriedForward;

                totalCredits = pageItems.TotalCredits;

                totalDebits = pageItems.TotalDebits;

                transactionsList.AddRange(pageItems.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageItems = await FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(pageIndex, pageSize, customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, false, serviceHeader);

                        if (pageItems != null && pageItems.PageCollection != null)
                        {
                            transactionsList.AddRange(pageItems.PageCollection);
                        }

                        ++pageIndex;
                    }
                }
            }

            var pageCollectionInfo = new PageCollectionInfo<GeneralLedgerTransaction>
            {
                PageCollection = transactionsList,
                BookBalanceBroughtFoward = openingBookBalance,
                BookBalanceCarriedForward = closingBookBalance,
                AvailableBalanceBroughtFoward = openingAvailableBalance,
                AvailableBalanceCarriedForward = closingAvailableBalance,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits
            };

            return pageCollectionInfo;
        }

        public PageCollectionInfo<JournalEntryDTO> FindReversibleJournalEntriesInPage(int pageIndex, int pageSize, int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalEntrySpecifications.ReversibleJournalEntriesWithDateRange(systemTransactionCode, startDate, endDate, text, journalEntryFilter, serviceHeader);

                ISpecification<JournalEntry> spec = filter;

                var sortFields = new List<string> { "CreatedDate" };

                var journalEntryPagedCollection = _journalEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalEntryPagedCollection != null)
                {
                    var pageCollection = journalEntryPagedCollection.PageCollection.ProjectedAsCollection<JournalEntryDTO>();

                    var itemsCount = journalEntryPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalEntryDTO>
                    {
                        PageCollection = pageCollection,
                        ItemsCount = itemsCount
                    };
                }
                else return null;
            }
        }

        public async Task<PageCollectionInfo<JournalEntryDTO>> FindReversibleJournalEntriesInPageAsync(int pageIndex, int pageSize, int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                PageCollectionInfo<JournalEntryDTO> source = null;

                var filter = JournalEntrySpecifications.ReversibleJournalEntriesWithDateRange(systemTransactionCode, startDate, endDate, text, journalEntryFilter, serviceHeader);

                ISpecification<JournalEntry> spec = filter;

                var sortFields = new List<string> { "CreatedDate" };

                var journalEntryPagedCollection = await _journalEntryRepository.AllMatchingPagedAsync(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalEntryPagedCollection != null)
                {
                    var pageCollection = journalEntryPagedCollection.PageCollection.ProjectedAsCollection<JournalEntryDTO>();

                    var itemsCount = journalEntryPagedCollection.ItemsCount;

                    source = new PageCollectionInfo<JournalEntryDTO>
                    {
                        PageCollection = pageCollection,
                        ItemsCount = itemsCount
                    };
                }

                return source;
            }
        }

        public PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsInPage(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader)
        {
            if (startDate != null && endDate != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalEntrySpecifications.JournalEntriesWithDateRange(startDate, endDate, text, journalEntryFilter);

                    ISpecification<JournalEntry> spec = filter;

                    var sortFields = new List<string> { "CreatedDate" };

                    var journalEntryPagedCollection = _journalEntryRepository.AllMatchingPaged<JournalEntrySummaryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (journalEntryPagedCollection != null)
                    {
                        var projection = journalEntryPagedCollection.PageCollection;

                        var pageItems = BuildGeneralLedgerTransactionsTrail(new Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>>(0m, projection), serviceHeader);

                        if (pageItems != null && pageItems.Any())
                        {
                            return new PageCollectionInfo<GeneralLedgerTransaction>
                            {
                                PageCollection = pageItems,
                                ItemsCount = journalEntryPagedCollection.ItemsCount
                            };
                        }
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                PageCollectionInfo<GeneralLedgerTransaction> source = null; /*page transactions*/

                var filter = JournalEntrySpecifications.JournalEntriesWithDateRange(startDate, endDate, text, journalEntryFilter);

                ISpecification<JournalEntry> spec = filter;

                var sortFields = new List<string> { "CreatedDate" };

                var journalEntryPagedCollection = await _journalEntryRepository.AllMatchingPagedAsync<JournalEntrySummaryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalEntryPagedCollection != null)
                {
                    var projection = journalEntryPagedCollection.PageCollection;

                    var pageItems = await BuildGeneralLedgerTransactionsTrailAsync(new Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>>(0m, projection), serviceHeader);

                    source = new PageCollectionInfo<GeneralLedgerTransaction>
                    {
                        PageCollection = pageItems,
                        ItemsCount = journalEntryPagedCollection.ItemsCount
                    };
                }

                return source;
            }
        }

        public PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeInPage(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            List<GeneralLedgerTransaction> pageItems = null;

            decimal openingBookBalance = 0m;

            decimal pageOpeningBookBalance = 0m;

            decimal closingBookBalance = 0m;

            decimal totalCredits = 0m;

            decimal totalDebits = 0m;

            int itemsCount = 0;

            // Tuple: Item1= [bal b/f], Item2 = [page transactions], Item3 = [bal c/f], Item4 = [total credits/debits] >> Izmoto 08.09.2017

            var tuple = FindJournalEntriesByChartOfAccountIdAndDateRange(pageIndex, pageSize, chartOfAccountId, startDate, endDate, text, journalEntryFilter, transactionDateFilter, tallyDebitsCredits, serviceHeader);

            if (tuple != null)
            {
                openingBookBalance = tuple.Item1;

                pageOpeningBookBalance = tuple.Item1;

                closingBookBalance = tuple.Item3;

                totalCredits = tuple.Item4.Item1;

                totalDebits = tuple.Item4.Item2;

                if (tuple.Item2 != null)
                {
                    itemsCount = tuple.Item2.ItemsCount;

                    if (pageIndex > 0)
                    {
                        // adjust page bal b/f
                        var cutOffDate = tuple.Item2.PageCollection[0].CreatedDate;

                        pageOpeningBookBalance = _sqlCommandAppService.FindGlAccountBalance(chartOfAccountId, cutOffDate, transactionDateFilter, serviceHeader);
                    }

                    pageItems = BuildGeneralLedgerTransactionsTrail(new Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>>(pageOpeningBookBalance, tuple.Item2.PageCollection), serviceHeader);
                }
            }

            return new PageCollectionInfo<GeneralLedgerTransaction> { PageCollection = pageItems, ItemsCount = itemsCount, BookBalanceBroughtFoward = openingBookBalance, BookBalanceCarriedForward = closingBookBalance, TotalCredits = totalCredits, TotalDebits = totalDebits };
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            List<GeneralLedgerTransaction> pageItems = null;

            decimal openingBookBalance = 0m;

            decimal pageOpeningBookBalance = 0m;

            decimal closingBookBalance = 0m;

            decimal totalCredits = 0m;

            decimal totalDebits = 0m;

            int itemsCount = 0;

            // Tuple: Item1= [bal b/f], Item2 = [page transactions], Item3 = [bal c/f], Item4 = [total credits/debits] >> Izmoto 08.09.2017

            var tuple = await FindJournalEntriesByChartOfAccountIdAndDateRangeAsync(pageIndex, pageSize, chartOfAccountId, startDate, endDate, text, journalEntryFilter, transactionDateFilter, tallyDebitsCredits, serviceHeader);

            if (tuple != null)
            {
                openingBookBalance = tuple.Item1;

                pageOpeningBookBalance = tuple.Item1;

                closingBookBalance = tuple.Item3;

                totalCredits = tuple.Item4.Item1;

                totalDebits = tuple.Item4.Item2;

                if (tuple.Item2 != null)
                {
                    itemsCount = tuple.Item2.ItemsCount;

                    if (pageIndex > 0)
                    {
                        // adjust page bal b/f
                        var cutOffDate = tuple.Item2.PageCollection[0].CreatedDate;

                        pageOpeningBookBalance = await _sqlCommandAppService.FindGlAccountBalanceAsync(chartOfAccountId, cutOffDate, transactionDateFilter, serviceHeader);
                    }

                    pageItems = await BuildGeneralLedgerTransactionsTrailAsync(new Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>>(pageOpeningBookBalance, tuple.Item2.PageCollection), serviceHeader);
                }
            }

            var pageCollectionInfo = new PageCollectionInfo<GeneralLedgerTransaction>
            {
                PageCollection = pageItems,
                ItemsCount = itemsCount,
                BookBalanceBroughtFoward = openingBookBalance,
                BookBalanceCarriedForward = closingBookBalance,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits
            };

            return pageCollectionInfo;
        }

        public PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPage(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            List<GeneralLedgerTransaction> pageItems = null;

            decimal openingBookBalance = 0m;

            decimal pageOpeningBookBalance = 0m;

            decimal closingBookBalance = 0m;

            decimal totalCredits = 0m;

            decimal totalDebits = 0m;

            int itemsCount = 0;

            // Tuple: Item1= [bal b/f], Item2 = [page transactions], Item3 = [bal c/f], Item4 = [total credits/debits] >> Izmoto 08.09.2017

            var tuple = FindJournalEntriesByChartOfAccountIdAndDateRangeAndTransactionCodeAndReference(pageIndex, pageSize, chartOfAccountId, startDate, endDate, transactionCode, reference, transactionDateFilter, tallyDebitsCredits, serviceHeader);

            if (tuple != null)
            {
                openingBookBalance = tuple.Item1;

                pageOpeningBookBalance = tuple.Item1;

                closingBookBalance = tuple.Item3;

                totalCredits = tuple.Item4.Item1;

                totalDebits = tuple.Item4.Item2;

                if (tuple.Item2 != null)
                {
                    itemsCount = tuple.Item2.ItemsCount;

                    if (pageIndex > 0)
                    {
                        // adjust page bal b/f
                        var cutOffDate = tuple.Item2.PageCollection[0].CreatedDate;

                        pageOpeningBookBalance = _sqlCommandAppService.FindGlAccountBalance(chartOfAccountId, cutOffDate, transactionDateFilter, serviceHeader);
                    }

                    pageItems = BuildGeneralLedgerTransactionsTrail(new Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>>(pageOpeningBookBalance, tuple.Item2.PageCollection), serviceHeader);
                }
            }

            return new PageCollectionInfo<GeneralLedgerTransaction> { PageCollection = pageItems, ItemsCount = itemsCount, BookBalanceBroughtFoward = openingBookBalance, BookBalanceCarriedForward = closingBookBalance, TotalCredits = totalCredits, TotalDebits = totalDebits };
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            List<GeneralLedgerTransaction> pageItems = null;

            decimal openingBookBalance = 0m;

            decimal pageOpeningBookBalance = 0m;

            decimal closingBookBalance = 0m;

            decimal totalCredits = 0m;

            decimal totalDebits = 0m;

            int itemsCount = 0;

            // Tuple: Item1= [bal b/f], Item2 = [page transactions], Item3 = [bal c/f], Item4 = [total credits/debits] >> Izmoto 08.09.2017

            var tuple = await FindJournalEntriesByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceAsync(pageIndex, pageSize, chartOfAccountId, startDate, endDate, transactionCode, reference, transactionDateFilter, tallyDebitsCredits, serviceHeader);

            openingBookBalance = tuple.Item1;

            pageOpeningBookBalance = tuple.Item1;

            closingBookBalance = tuple.Item3;

            totalCredits = tuple.Item4.Item1;

            totalDebits = tuple.Item4.Item2;

            itemsCount = tuple.Item2.ItemsCount;

            if (pageIndex > 0)
            {
                // adjust page bal b/f
                var cutOffDate = tuple.Item2.PageCollection[0].CreatedDate;

                pageOpeningBookBalance = await _sqlCommandAppService.FindGlAccountBalanceAsync(chartOfAccountId, cutOffDate, transactionDateFilter, serviceHeader);
            }

            pageItems = await BuildGeneralLedgerTransactionsTrailAsync(new Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>>(pageOpeningBookBalance, tuple.Item2.PageCollection), serviceHeader);

            var pageCollectionInfo = new PageCollectionInfo<GeneralLedgerTransaction>
            {
                PageCollection = pageItems,
                ItemsCount = itemsCount,
                BookBalanceBroughtFoward = openingBookBalance,
                BookBalanceCarriedForward = closingBookBalance,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits
            };

            return pageCollectionInfo;
        }

        public PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPage(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            List<GeneralLedgerTransaction> pageItems = null;

            int itemsCount = 0;

            decimal openingAvailableBalance = 0m;

            decimal pageOpeningAvailableBalance = 0m;

            decimal openingBookBalance = 0m;

            decimal pageOpeningBookBalance = 0m;

            decimal closingAvailableBalance = 0m;

            decimal closingBookBalance = 0m;

            decimal totalCredits = 0m;

            decimal totalDebits = 0m;

            // Tuple: Item1= [available balance bf], Item2= [book balance bf] Item3 = [page transactions] Item4= [available balance cf], Item5= [book balance cf], Item6= [total_credits + total_debits]>> Izmoto 19.08.2017

            var tuple = FindJournalEntriesByCustomerAccountIdAndDateRangeInPage(pageIndex, pageSize, customerAccountDTO, startDate, endDate, text, journalEntryFilter, tallyDebitsCredits, serviceHeader);

            if (tuple != null)
            {
                openingAvailableBalance = tuple.Item1;

                pageOpeningAvailableBalance = openingAvailableBalance;

                openingBookBalance = tuple.Item2;

                pageOpeningBookBalance = openingBookBalance;

                closingAvailableBalance = tuple.Item4;

                closingBookBalance = tuple.Item5;

                totalCredits = tuple.Item6.Item1;

                totalDebits = tuple.Item6.Item2;

                if (tuple.Item3 != null && tuple.Item3.PageCollection.Any())
                {
                    itemsCount = tuple.Item3.ItemsCount;

                    if (pageIndex > 0)
                    {
                        // adjust page bal b/f

                        var cutOffDate = tuple.Item3.PageCollection[0].CreatedDate;

                        switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                        {
                            case ProductCode.Savings:

                                switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                                {
                                    case CustomerAccountStatementType.FixedDepositStatement:

                                        pageOpeningBookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 3, cutOffDate, serviceHeader);

                                        break;
                                    case CustomerAccountStatementType.ChequeDepositStatement:

                                        pageOpeningBookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, cutOffDate, serviceHeader);

                                        break;
                                    case CustomerAccountStatementType.PrincipalStatement:
                                    default:

                                        pageOpeningAvailableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(customerAccountDTO, cutOffDate, serviceHeader);

                                        pageOpeningBookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, cutOffDate, serviceHeader);

                                        break;
                                }

                                break;
                            case ProductCode.Loan:

                                switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                                {
                                    case CustomerAccountStatementType.InterestStatement:

                                        pageOpeningBookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, cutOffDate, serviceHeader);

                                        break;
                                    case CustomerAccountStatementType.PrincipalStatement:
                                    default:

                                        pageOpeningBookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, cutOffDate, serviceHeader);

                                        break;
                                }

                                break;
                            case ProductCode.Investment:

                                pageOpeningBookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, cutOffDate, serviceHeader);

                                break;
                            default:
                                break;
                        }
                    }

                    pageItems = BuildCustomerAccountTransactionsTrail(customerAccountDTO, new Tuple<decimal, decimal, IEnumerable<JournalEntrySummaryDTO>>(pageOpeningAvailableBalance, pageOpeningBookBalance, tuple.Item3.PageCollection), serviceHeader);
                }
            }

            return new PageCollectionInfo<GeneralLedgerTransaction> { PageCollection = pageItems, ItemsCount = itemsCount, BookBalanceBroughtFoward = openingBookBalance, BookBalanceCarriedForward = closingBookBalance, AvailableBalanceBroughtFoward = openingAvailableBalance, AvailableBalanceCarriedForward = closingAvailableBalance, TotalCredits = totalCredits, TotalDebits = totalDebits };
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            List<GeneralLedgerTransaction> pageItems = null;

            int itemsCount = 0;

            decimal openingAvailableBalance = 0m;

            decimal pageOpeningAvailableBalance = 0m;

            decimal openingBookBalance = 0m;

            decimal pageOpeningBookBalance = 0m;

            decimal closingAvailableBalance = 0m;

            decimal closingBookBalance = 0m;

            decimal totalCredits = 0m;

            decimal totalDebits = 0m;

            // Tuple: Item1= [available balance bf], Item2= [book balance bf] Item3 = [page transactions] Item4= [available balance cf], Item5= [book balance cf], Item6= [total_credits + total_debits]>> Izmoto 19.08.2017

            var tuple = await FindJournalEntriesByCustomerAccountIdAndDateRangeInPageAsync(pageIndex, pageSize, customerAccountDTO, startDate, endDate, text, journalEntryFilter, tallyDebitsCredits, serviceHeader);

            openingAvailableBalance = tuple.Item1;

            pageOpeningAvailableBalance = openingAvailableBalance;

            openingBookBalance = tuple.Item2;

            pageOpeningBookBalance = openingBookBalance;

            closingAvailableBalance = tuple.Item4;

            closingBookBalance = tuple.Item5;

            totalCredits = tuple.Item6.Item1;

            totalDebits = tuple.Item6.Item2;

            itemsCount = tuple.Item3.ItemsCount;

            if (pageIndex > 0)
            {
                // adjust page bal b/f

                var cutOffDate = tuple.Item3.PageCollection[0].CreatedDate;

                switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                {
                    case ProductCode.Savings:

                        switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                        {
                            case CustomerAccountStatementType.FixedDepositStatement:

                                pageOpeningBookBalance = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 3, cutOffDate, serviceHeader);

                                break;
                            case CustomerAccountStatementType.ChequeDepositStatement:

                                pageOpeningBookBalance = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, cutOffDate, serviceHeader);

                                break;
                            case CustomerAccountStatementType.PrincipalStatement:
                            default:

                                pageOpeningAvailableBalance = await _sqlCommandAppService.FindCustomerAccountAvailableBalanceAsync(customerAccountDTO, cutOffDate, serviceHeader);

                                pageOpeningBookBalance = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, cutOffDate, serviceHeader);

                                break;
                        }

                        break;
                    case ProductCode.Loan:

                        switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                        {
                            case CustomerAccountStatementType.InterestStatement:

                                pageOpeningBookBalance = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 2, cutOffDate, serviceHeader);

                                break;
                            case CustomerAccountStatementType.PrincipalStatement:
                            default:

                                pageOpeningBookBalance = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, cutOffDate, serviceHeader);

                                break;
                        }

                        break;
                    case ProductCode.Investment:

                        pageOpeningBookBalance = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, cutOffDate, serviceHeader);

                        break;
                    default:
                        break;
                }
            }

            pageItems = await BuildCustomerAccountTransactionsTrailAsync(customerAccountDTO, new Tuple<decimal, decimal, IEnumerable<JournalEntrySummaryDTO>>(pageOpeningAvailableBalance, pageOpeningBookBalance, tuple.Item3.PageCollection), serviceHeader);

            var pageCollectionInfo = new PageCollectionInfo<GeneralLedgerTransaction>
            {
                PageCollection = pageItems,
                ItemsCount = itemsCount,
                BookBalanceBroughtFoward = openingBookBalance,
                BookBalanceCarriedForward = closingBookBalance,
                AvailableBalanceBroughtFoward = openingAvailableBalance,
                AvailableBalanceCarriedForward = closingAvailableBalance,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits
            };

            return pageCollectionInfo;
        }

        private Tuple<decimal, decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, decimal, Tuple<decimal, decimal>> FindJournalEntriesByCustomerAccountIdAndDateRangeInPage(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            decimal source1 = 0m; /*Opening Available Balance*/

            decimal source2 = 0m; /*Opening Book Balance*/

            PageCollectionInfo<JournalEntrySummaryDTO> source3 = null; /*other tx*/

            decimal source4 = 0m;/*Closing Available Balance*/

            decimal source5 = 0m; /*Closing Book Balance*/

            decimal source6 = 0m; /*total credits*/

            decimal source7 = 0m; /*total debits*/

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<JournalEntry> otherTransactionsSpec = null;

                switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                {
                    case ProductCode.Savings:

                        switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                        {
                            case CustomerAccountStatementType.FixedDepositStatement:

                                source2 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 3, startDate.AddDays(-1), serviceHeader);

                                source5 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 3, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                var fixedDepositControlAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.FixedDeposit, serviceHeader);

                                if (fixedDepositControlAccountId != null && fixedDepositControlAccountId != Guid.Empty)
                                    otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, fixedDepositControlAccountId);

                                break;
                            case CustomerAccountStatementType.ChequeDepositStatement:

                                source2 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                                source5 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                var externalChequesControlAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesControl, serviceHeader);

                                if (externalChequesControlAccountId != null && externalChequesControlAccountId != Guid.Empty)
                                    otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, externalChequesControlAccountId);

                                break;
                            case CustomerAccountStatementType.PrincipalStatement:
                            default:

                                source1 = _sqlCommandAppService.FindCustomerAccountAvailableBalance(customerAccountDTO, startDate.AddDays(-1), serviceHeader);

                                source2 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                                source4 = _sqlCommandAppService.FindCustomerAccountAvailableBalance(customerAccountDTO, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                source5 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId);

                                break;
                        }

                        break;
                    case ProductCode.Loan:

                        switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                        {
                            case CustomerAccountStatementType.InterestStatement:

                                source2 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, startDate.AddDays(-1), serviceHeader);

                                source5 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId);

                                break;
                            case CustomerAccountStatementType.PrincipalStatement:
                            default:

                                source2 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                                source5 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId);

                                break;
                        }

                        break;
                    case ProductCode.Investment:

                        source2 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                        source5 = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                        otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId);

                        break;
                    default:
                        break;
                }

                if (otherTransactionsSpec != null)
                {
                    var sortFields = new List<string> { "CreatedDate" };

                    source3 = _journalEntryRepository.AllMatchingPaged<JournalEntrySummaryDTO>(otherTransactionsSpec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (tallyDebitsCredits)
                    {
                        var closingBalanceJournalEntries = _journalEntryRepository.AllMatching<JournalEntrySummaryDTO>(otherTransactionsSpec, serviceHeader);

                        if (closingBalanceJournalEntries != null)
                        {
                            // total credits
                            source6 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal creditAmount = (element.Amount > 0m) ? element.Amount : 0m;

                                return result + creditAmount;
                            });

                            // total debits
                            source7 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal debitAmount = (element.Amount < 0m) ? element.Amount * -1 : 0m;

                                return result + debitAmount;
                            });
                        }
                    }
                }
            }

            return new Tuple<decimal, decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, decimal, Tuple<decimal, decimal>>(source1, source2, source3, source4, source5, new Tuple<decimal, decimal>(source6, source7));
        }

        private async Task<Tuple<decimal, decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, decimal, Tuple<decimal, decimal>>> FindJournalEntriesByCustomerAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                decimal source1 = 0m; /*Opening Available Balance*/

                decimal source2 = 0m; /*Opening Book Balance*/

                PageCollectionInfo<JournalEntrySummaryDTO> source3 = null; /*other tx*/

                decimal source4 = 0m;/*Closing Available Balance*/

                decimal source5 = 0m; /*Closing Book Balance*/

                decimal source6 = 0m; /*total credits*/

                decimal source7 = 0m; /*total debits*/

                ISpecification<JournalEntry> otherTransactionsSpec = null;

                switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                {
                    case ProductCode.Savings:

                        switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                        {
                            case CustomerAccountStatementType.FixedDepositStatement:

                                source2 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 3, startDate.AddDays(-1), serviceHeader);

                                source5 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 3, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                var fixedDepositControlAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.FixedDeposit, serviceHeader);

                                if (fixedDepositControlAccountId != null && fixedDepositControlAccountId != Guid.Empty)
                                    otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, fixedDepositControlAccountId);

                                break;
                            case CustomerAccountStatementType.ChequeDepositStatement:

                                source2 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                                source5 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                var externalChequesControlAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesControl, serviceHeader);

                                if (externalChequesControlAccountId != null && externalChequesControlAccountId != Guid.Empty)
                                    otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, externalChequesControlAccountId);

                                break;
                            case CustomerAccountStatementType.PrincipalStatement:
                            default:

                                source1 = await _sqlCommandAppService.FindCustomerAccountAvailableBalanceAsync(customerAccountDTO, startDate.AddDays(-1), serviceHeader);

                                source2 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                                source4 = await _sqlCommandAppService.FindCustomerAccountAvailableBalanceAsync(customerAccountDTO, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                source5 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId);

                                break;
                        }

                        break;
                    case ProductCode.Loan:

                        switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                        {
                            case CustomerAccountStatementType.InterestStatement:

                                source2 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 2, startDate.AddDays(-1), serviceHeader);

                                source5 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 2, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId);

                                break;
                            case CustomerAccountStatementType.PrincipalStatement:
                            default:
                                source2 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                                source5 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId);

                                break;
                        }

                        break;
                    case ProductCode.Investment:

                        source2 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, startDate.AddDays(-1), serviceHeader);

                        source5 = await _sqlCommandAppService.FindCustomerAccountBookBalanceAsync(customerAccountDTO, 1, UberUtil.AdjustTimeSpan(endDate), serviceHeader);

                        otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithCustomerAccountIdAndDateRange(customerAccountDTO.Id, startDate, endDate, text, journalEntryFilter, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId);

                        break;
                    default:
                        break;
                }

                if (otherTransactionsSpec != null)
                {
                    var sortFields = new List<string> { "CreatedDate" };

                    source3 = await _journalEntryRepository.AllMatchingPagedAsync<JournalEntrySummaryDTO>(otherTransactionsSpec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (tallyDebitsCredits)
                    {
                        var closingBalanceJournalEntries = await _journalEntryRepository.AllMatchingAsync<JournalEntrySummaryDTO>(otherTransactionsSpec, serviceHeader);

                        if (closingBalanceJournalEntries != null)
                        {
                            // total credits
                            source6 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal creditAmount = (element.Amount > 0m) ? element.Amount : 0m;

                                return result + creditAmount;
                            });

                            // total debits
                            source7 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal debitAmount = (element.Amount < 0m) ? element.Amount * -1 : 0m;

                                return result + debitAmount;
                            });
                        }
                    }
                }

                var resultTuple = new Tuple<decimal, decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, decimal, Tuple<decimal, decimal>>(source1, source2, source3, source4, source5, new Tuple<decimal, decimal>(source6, source7));

                return resultTuple;
            }

        }

        private Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>> FindJournalEntriesByChartOfAccountIdAndDateRange(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            decimal source1 = 0m; /*opening book balance*/

            PageCollectionInfo<JournalEntrySummaryDTO> source2 = null; /*page transactions*/

            decimal source3 = 0m; /*closing book balance*/

            decimal source4 = 0m; /*total credits*/

            decimal source5 = 0m; /*total debits*/

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<JournalEntry> otherTransactionsSpec = null;

                source1 = _sqlCommandAppService.FindGlAccountBalance(chartOfAccountId, startDate.AddDays(-1), transactionDateFilter, serviceHeader);

                source3 = _sqlCommandAppService.FindGlAccountBalance(chartOfAccountId, UberUtil.AdjustTimeSpan(endDate), transactionDateFilter, serviceHeader);

                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithChartOfAccountIdAndDateRange(chartOfAccountId, startDate, endDate, text, journalEntryFilter, transactionDateFilter);

                if (otherTransactionsSpec != null)
                {
                    var sortFields = new List<string> { "CreatedDate" };

                    source2 = _journalEntryRepository.AllMatchingPaged<JournalEntrySummaryDTO>(otherTransactionsSpec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (tallyDebitsCredits)
                    {
                        var closingBalanceJournalEntries = _journalEntryRepository.AllMatching<JournalEntrySummaryDTO>(otherTransactionsSpec, serviceHeader);

                        if (closingBalanceJournalEntries != null)
                        {
                            // total credits
                            source4 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal creditAmount = (element.Amount > 0m) ? element.Amount : 0m;

                                return result + creditAmount;
                            });

                            // total debits
                            source5 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal debitAmount = (element.Amount < 0m) ? element.Amount * -1 : 0m;

                                return result + debitAmount;
                            });
                        }
                    }
                }
            }

            return new Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>>(source1, source2, source3, new Tuple<decimal, decimal>(source4, source5));
        }

        private async Task<Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>>> FindJournalEntriesByChartOfAccountIdAndDateRangeAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                decimal source1 = 0m; /*opening book balance*/

                PageCollectionInfo<JournalEntrySummaryDTO> source2 = null; /*page transactions*/

                decimal source3 = 0m; /*closing book balance*/

                decimal source4 = 0m; /*total credits*/

                decimal source5 = 0m; /*total debits*/

                ISpecification<JournalEntry> otherTransactionsSpec = null;

                source1 = await _sqlCommandAppService.FindGlAccountBalanceAsync(chartOfAccountId, startDate.AddDays(-1), transactionDateFilter, serviceHeader);

                source3 = await _sqlCommandAppService.FindGlAccountBalanceAsync(chartOfAccountId, UberUtil.AdjustTimeSpan(endDate), transactionDateFilter, serviceHeader);

                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithChartOfAccountIdAndDateRange(chartOfAccountId, startDate, endDate, text, journalEntryFilter, transactionDateFilter);

                if (otherTransactionsSpec != null)
                {
                    var sortFields = new List<string> { "CreatedDate" };

                    var journalEntryPagedCollection = await _journalEntryRepository.AllMatchingPagedAsync<JournalEntrySummaryDTO>(otherTransactionsSpec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (journalEntryPagedCollection != null)
                    {
                        var pageCollection = journalEntryPagedCollection.PageCollection;

                        var itemsCount = journalEntryPagedCollection.ItemsCount;

                        source2 = new PageCollectionInfo<JournalEntrySummaryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }

                    if (tallyDebitsCredits)
                    {
                        var closingBalanceJournalEntries = await _journalEntryRepository.AllMatchingAsync<JournalEntrySummaryDTO>(otherTransactionsSpec, serviceHeader);

                        if (closingBalanceJournalEntries != null)
                        {
                            // total credits
                            source4 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal creditAmount = (element.Amount > 0m) ? element.Amount : 0m;

                                return result + creditAmount;
                            });

                            // total debits
                            source5 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal debitAmount = (element.Amount < 0m) ? element.Amount * -1 : 0m;

                                return result + debitAmount;
                            });
                        }
                    }
                }

                var resultTuple = new Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>>(source1, source2, source3, new Tuple<decimal, decimal>(source4, source5));

                return resultTuple;
            }
        }

        private Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>> FindJournalEntriesByChartOfAccountIdAndDateRangeAndTransactionCodeAndReference(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            decimal source1 = 0m; /*opening book balance*/

            PageCollectionInfo<JournalEntrySummaryDTO> source2 = null; /*page transactions*/

            decimal source3 = 0m; /*closing book balance*/

            decimal source4 = 0m; /*total credits*/

            decimal source5 = 0m; /*total debits*/

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<JournalEntry> otherTransactionsSpec = null;

                source1 = _sqlCommandAppService.FindGlAccountBalance(chartOfAccountId, startDate.AddDays(-1), transactionDateFilter, serviceHeader);

                source3 = _sqlCommandAppService.FindGlAccountBalance(chartOfAccountId, UberUtil.AdjustTimeSpan(endDate), transactionDateFilter, serviceHeader);

                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithChartOfAccountIdAndDateRange(chartOfAccountId, startDate, endDate, transactionCode, reference, transactionDateFilter);

                if (otherTransactionsSpec != null)
                {
                    var sortFields = new List<string> { "CreatedDate" };

                    source2 = _journalEntryRepository.AllMatchingPaged<JournalEntrySummaryDTO>(otherTransactionsSpec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (tallyDebitsCredits)
                    {
                        var closingBalanceJournalEntries = _journalEntryRepository.AllMatching<JournalEntrySummaryDTO>(otherTransactionsSpec, serviceHeader);

                        if (closingBalanceJournalEntries != null)
                        {
                            // total credits
                            source4 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal creditAmount = (element.Amount > 0m) ? element.Amount : 0m;

                                return result + creditAmount;
                            });

                            // total debits
                            source5 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal debitAmount = (element.Amount < 0m) ? element.Amount * -1 : 0m;

                                return result + debitAmount;
                            });
                        }
                    }
                }
            }

            return new Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>>(source1, source2, source3, new Tuple<decimal, decimal>(source4, source5));
        }

        private async Task<Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>>> FindJournalEntriesByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                decimal source1 = 0m; /*opening book balance*/

                PageCollectionInfo<JournalEntrySummaryDTO> source2 = null; /*page transactions*/

                decimal source3 = 0m; /*closing book balance*/

                decimal source4 = 0m; /*total credits*/

                decimal source5 = 0m; /*total debits*/

                ISpecification<JournalEntry> otherTransactionsSpec = null;

                source1 = await _sqlCommandAppService.FindGlAccountBalanceAsync(chartOfAccountId, startDate.AddDays(-1), transactionDateFilter, serviceHeader);

                source3 = await _sqlCommandAppService.FindGlAccountBalanceAsync(chartOfAccountId, UberUtil.AdjustTimeSpan(endDate), transactionDateFilter, serviceHeader);

                otherTransactionsSpec = JournalEntrySpecifications.JournalEntriesWithChartOfAccountIdAndDateRange(chartOfAccountId, startDate, endDate, transactionCode, reference, transactionDateFilter);

                if (otherTransactionsSpec != null)
                {
                    var sortFields = new List<string> { "CreatedDate" };

                    var journalEntryPagedCollection = await _journalEntryRepository.AllMatchingPagedAsync<JournalEntrySummaryDTO>(otherTransactionsSpec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (journalEntryPagedCollection != null)
                    {
                        var pageCollection = journalEntryPagedCollection.PageCollection;

                        var itemsCount = journalEntryPagedCollection.ItemsCount;

                        source2 = new PageCollectionInfo<JournalEntrySummaryDTO>
                        {
                            PageCollection = pageCollection,
                            ItemsCount = itemsCount
                        };
                    }

                    if (tallyDebitsCredits)
                    {
                        var closingBalanceJournalEntries = await _journalEntryRepository.AllMatchingAsync<JournalEntrySummaryDTO>(otherTransactionsSpec, serviceHeader);

                        if (closingBalanceJournalEntries != null)
                        {
                            // total credits
                            source4 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal creditAmount = (element.Amount > 0m) ? element.Amount : 0m;

                                return result + creditAmount;
                            });

                            // total debits
                            source5 = closingBalanceJournalEntries.Aggregate(0m, (result, element) =>
                            {
                                decimal debitAmount = (element.Amount < 0m) ? element.Amount * -1 : 0m;

                                return result + debitAmount;
                            });
                        }
                    }
                }

                var resultTuple = new Tuple<decimal, PageCollectionInfo<JournalEntrySummaryDTO>, decimal, Tuple<decimal, decimal>>(source1, source2, source3, new Tuple<decimal, decimal>(source4, source5));

                return resultTuple;
            }
        }

        private List<GeneralLedgerTransaction> BuildGeneralLedgerTransactionsTrail(Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>> tuple, ServiceHeader serviceHeader)
        {
            var precedentTask = Task<List<GeneralLedgerTransaction>>.Factory.StartNew(() =>
            {
                return new List<GeneralLedgerTransaction> { };
            });

            return precedentTask.ContinueWith(antecedentTask =>
            {
                var result = antecedentTask.Result;

                if (tuple.Item2 != null)
                {
                    var source = tuple.Item2;

                    GeneralLedgerTransaction[] generalLedgerTransactions = new GeneralLedgerTransaction[source.Count()];

                    Parallel.ForEach(source, (line, state, index) =>
                    {
                        GeneralLedgerTransaction glTransaction =
                            new GeneralLedgerTransaction
                            {
                                Id = line.Id,
                                JournalId = line.JournalId,
                                JournalParentId = line.JournalParentId,
                                JournalPrimaryDescription = line.JournalPrimaryDescription,
                                JournalSecondaryDescription = line.JournalSecondaryDescription,
                                JournalReference = line.JournalReference,
                                JournalTransactionCode = line.JournalTransactionCode,
                                JournalIsLocked = line.JournalIsLocked,
                                JournalValueDate = line.ValueDate,
                                JournalCreatedDate = line.CreatedDate,
                                BranchDescription = line.JournalBranchDescription,
                                GLAccountId = line.ChartOfAccountId,
                                CustomerAccountId = line.CustomerAccountId,
                                ContraGLAccountId = line.ContraChartOfAccountId,
                                Credit = (line.Amount > 0m) ? line.Amount : 0m,
                                Debit = (line.Amount < 0m) ? line.Amount * -1 : 0m,
                                ApplicationUserName = line.JournalApplicationUserName,
                                EnvironmentUserName = line.JournalEnvironmentUserName,
                                EnvironmentMachineName = line.JournalEnvironmentMachineName,
                                EnvironmentDomainName = line.JournalEnvironmentDomainName,
                                EnvironmentOSVersion = line.JournalEnvironmentOSVersion,
                                EnvironmentMACAddress = line.JournalEnvironmentMACAddress,
                                EnvironmentMotherboardSerialNumber = line.JournalEnvironmentMotherboardSerialNumber,
                                EnvironmentProcessorId = line.JournalEnvironmentProcessorId,
                                EnvironmentIPAddress = line.JournalEnvironmentIPAddress,
                            };

                        generalLedgerTransactions[index] = glTransaction;
                    });

                    if (generalLedgerTransactions != null)
                    {
                        Flatten(generalLedgerTransactions, serviceHeader);

                        generalLedgerTransactions = generalLedgerTransactions.Where(x => x != null).OrderBy(x => x.JournalCreatedDate).ToArray();

                        decimal cumulativeBookBalance = 0m;

                        cumulativeBookBalance += tuple.Item1;

                        Array.ForEach(generalLedgerTransactions, (line) =>
                        {
                            var netBookValue = 0m;

                            netBookValue = (line.Credit + (line.Debit * -1));

                            cumulativeBookBalance += netBookValue;

                            line.BookBalance = cumulativeBookBalance;
                        });

                        result.AddRange(generalLedgerTransactions);
                    }
                }

                return result;

            }).Result;
        }

        private async Task<List<GeneralLedgerTransaction>> BuildGeneralLedgerTransactionsTrailAsync(Tuple<decimal, IEnumerable<JournalEntrySummaryDTO>> tuple, ServiceHeader serviceHeader)
        {
            var result = new List<GeneralLedgerTransaction> { };

            if (tuple.Item2 != null)
            {
                var source = tuple.Item2;

                GeneralLedgerTransaction[] generalLedgerTransactions = new GeneralLedgerTransaction[source.Count()];

                Parallel.ForEach(source, (line, state, index) =>
                {
                    GeneralLedgerTransaction glTransaction =
                        new GeneralLedgerTransaction
                        {
                            Id = line.Id,
                            JournalId = line.JournalId,
                            JournalParentId = line.JournalParentId,
                            JournalPrimaryDescription = line.JournalPrimaryDescription,
                            JournalSecondaryDescription = line.JournalSecondaryDescription,
                            JournalReference = line.JournalReference,
                            JournalTransactionCode = line.JournalTransactionCode,
                            JournalIsLocked = line.JournalIsLocked,
                            JournalValueDate = line.ValueDate,
                            JournalCreatedDate = line.CreatedDate,
                            BranchDescription = line.JournalBranchDescription,
                            GLAccountId = line.ChartOfAccountId,
                            CustomerAccountId = line.CustomerAccountId,
                            ContraGLAccountId = line.ContraChartOfAccountId,
                            //Credit = (line.Amount > 0m) ? line.Amount : 0m,
                            //Debit = (line.Amount < 0m) ? line.Amount * -1 : 0m,
                            Debit = (line.Amount > 0m) ? line.Amount : 0m,
                            Credit = (line.Amount < 0m) ? line.Amount * -1 : 0m,
                            ApplicationUserName = line.JournalApplicationUserName,
                            EnvironmentUserName = line.JournalEnvironmentUserName,
                            EnvironmentMachineName = line.JournalEnvironmentMachineName,
                            EnvironmentDomainName = line.JournalEnvironmentDomainName,
                            EnvironmentOSVersion = line.JournalEnvironmentOSVersion,
                            EnvironmentMACAddress = line.JournalEnvironmentMACAddress,
                            EnvironmentMotherboardSerialNumber = line.JournalEnvironmentMotherboardSerialNumber,
                            EnvironmentProcessorId = line.JournalEnvironmentProcessorId,
                            EnvironmentIPAddress = line.JournalEnvironmentIPAddress,
                        };

                    generalLedgerTransactions[index] = glTransaction;
                });

                if (generalLedgerTransactions != null)
                {
                    await FlattenAsync(generalLedgerTransactions, serviceHeader);

                    generalLedgerTransactions = generalLedgerTransactions.Where(x => x != null).OrderBy(x => x.JournalCreatedDate).ToArray();

                    decimal cumulativeBookBalance = 0m;

                    cumulativeBookBalance += tuple.Item1;

                    Array.ForEach(generalLedgerTransactions, (line) =>
                    {
                        var netBookValue = 0m;

                        //netBookValue = (line.Credit + (line.Debit * -1));

                        netBookValue = (line.Debit + (line.Credit * -1));

                        cumulativeBookBalance += netBookValue;

                        line.BookBalance = cumulativeBookBalance;

                        line.RunningBalance = line.BookBalance;
                    });

                    result.AddRange(generalLedgerTransactions);
                }
            }

            return result;
        }

        private List<GeneralLedgerTransaction> BuildCustomerAccountTransactionsTrail(CustomerAccountDTO customerAccountDTO, Tuple<decimal, decimal, IEnumerable<JournalEntrySummaryDTO>> tuple, ServiceHeader serviceHeader)
        {
            // Tuple: Item1 = [available balance], Item2= [book balance] Item3 = [page transactions] >> Izmoto 12.06.2015

            var precedentTask = Task<List<GeneralLedgerTransaction>>.Factory.StartNew(() =>
            {
                return new List<GeneralLedgerTransaction> { };
            });

            return precedentTask.ContinueWith(antecedentTask =>
            {
                var result = antecedentTask.Result;

                if (tuple.Item3 != null)
                {
                    var source = tuple.Item3;

                    // Step 1: Work out transactions >> sequence does not matter, izmoto 25.04.2012!

                    GeneralLedgerTransaction[] generalLedgerTransactions = new GeneralLedgerTransaction[source.Count()];

                    Parallel.ForEach(source, (line, state, index) =>
                    {
                        GeneralLedgerTransaction glTransaction =
                            new GeneralLedgerTransaction
                            {
                                Id = line.Id,
                                JournalId = line.JournalId,
                                JournalParentId = line.JournalParentId,
                                JournalPrimaryDescription = line.JournalPrimaryDescription,
                                JournalSecondaryDescription = line.JournalSecondaryDescription,
                                JournalReference = line.JournalReference,
                                JournalTransactionCode = line.JournalTransactionCode,
                                JournalIsLocked = line.JournalIsLocked,
                                JournalValueDate = line.ValueDate,
                                JournalCreatedDate = line.CreatedDate,
                                BranchDescription = line.JournalBranchDescription,
                                GLAccountId = line.ChartOfAccountId,
                                CustomerAccountId = line.CustomerAccountId,
                                ContraGLAccountId = line.ContraChartOfAccountId,
                                Credit = (line.Amount > 0m) ? line.Amount : 0m,
                                Debit = (line.Amount < 0m) ? line.Amount * -1 : 0m,
                                ApplicationUserName = line.JournalApplicationUserName,
                                EnvironmentUserName = line.JournalEnvironmentUserName,
                                EnvironmentMachineName = line.JournalEnvironmentMachineName,
                                EnvironmentDomainName = line.JournalEnvironmentDomainName,
                                EnvironmentOSVersion = line.JournalEnvironmentOSVersion,
                                EnvironmentMACAddress = line.JournalEnvironmentMACAddress,
                                EnvironmentMotherboardSerialNumber = line.JournalEnvironmentMotherboardSerialNumber,
                                EnvironmentProcessorId = line.JournalEnvironmentProcessorId,
                                EnvironmentIPAddress = line.JournalEnvironmentIPAddress,
                            };

                        generalLedgerTransactions[index] = glTransaction;
                    });

                    // Step 2: Work out cumulative balance >> sequence matters, izmoto 25.04.2012!

                    if (generalLedgerTransactions != null)
                    {
                        Flatten(generalLedgerTransactions, serviceHeader);

                        generalLedgerTransactions = generalLedgerTransactions.Where(x => x != null).OrderBy(x => x.JournalCreatedDate).ToArray();

                        decimal cumulativeAvailableBalance = 0m;
                        decimal cumulativeBookBalance = 0m;

                        if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Savings))
                        {
                            cumulativeAvailableBalance += tuple.Item1;
                        }
                        else if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan, (int)ProductCode.Investment))
                        {
                            cumulativeBookBalance += tuple.Item2;
                        }

                        Array.ForEach(generalLedgerTransactions, (line) =>
                        {
                            var netAvailableValue = 0m;

                            var netBookValue = 0m;

                            switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                            {
                                case ProductCode.Savings:

                                    switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                                    {
                                        case CustomerAccountStatementType.FixedDepositStatement:
                                        case CustomerAccountStatementType.ChequeDepositStatement:

                                            netBookValue = (line.Credit + (line.Debit * -1));

                                            break;
                                        case CustomerAccountStatementType.PrincipalStatement:
                                        default:

                                            netAvailableValue = (line.Credit + (line.Debit * -1));

                                            break;
                                    }

                                    break;
                                case ProductCode.Loan:
                                case ProductCode.Investment:

                                    netBookValue = (line.Credit + (line.Debit * -1));

                                    break;
                                default:
                                    break;
                            }

                            cumulativeAvailableBalance += netAvailableValue;
                            cumulativeBookBalance += netBookValue;

                            line.AvailableBalance = cumulativeAvailableBalance;
                            line.BookBalance = cumulativeBookBalance;

                            if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Savings))
                            {
                                if (customerAccountDTO.CustomerAccountStatementType.In((int)CustomerAccountStatementType.ChequeDepositStatement, (int)CustomerAccountStatementType.FixedDepositStatement))
                                {
                                    line.RunningBalance = line.BookBalance;
                                }
                                else line.RunningBalance = line.AvailableBalance;
                            }
                            else if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan, (int)ProductCode.Investment))
                            {
                                line.RunningBalance = line.BookBalance;
                            }
                        });

                        result.AddRange(generalLedgerTransactions);
                    }
                }

                return result;

            }).Result;
        }

        private async Task<List<GeneralLedgerTransaction>> BuildCustomerAccountTransactionsTrailAsync(CustomerAccountDTO customerAccountDTO, Tuple<decimal, decimal, IEnumerable<JournalEntrySummaryDTO>> tuple, ServiceHeader serviceHeader)
        {
            // Tuple: Item1 = [available balance], Item2= [book balance] Item3 = [page transactions] >> Izmoto 12.06.2015

            var result = new List<GeneralLedgerTransaction> { };

            if (tuple.Item3 != null)
            {
                var source = tuple.Item3;

                // Step 1: Work out transactions >> sequence does not matter, izmoto 25.04.2012!

                GeneralLedgerTransaction[] generalLedgerTransactions = new GeneralLedgerTransaction[source.Count()];

                Parallel.ForEach(source, (line, state, index) =>
                {
                    GeneralLedgerTransaction glTransaction =
                        new GeneralLedgerTransaction
                        {
                            Id = line.Id,
                            JournalId = line.JournalId,
                            JournalParentId = line.JournalParentId,
                            JournalPrimaryDescription = line.JournalPrimaryDescription,
                            JournalSecondaryDescription = line.JournalSecondaryDescription,
                            JournalReference = line.JournalReference,
                            JournalTransactionCode = line.JournalTransactionCode,
                            JournalIsLocked = line.JournalIsLocked,
                            JournalValueDate = line.ValueDate,
                            JournalCreatedDate = line.CreatedDate,
                            BranchDescription = line.JournalBranchDescription,
                            GLAccountId = line.ChartOfAccountId,
                            CustomerAccountId = line.CustomerAccountId,
                            ContraGLAccountId = line.ContraChartOfAccountId,
                            Credit = (line.Amount > 0m) ? line.Amount : 0m,
                            Debit = (line.Amount < 0m) ? line.Amount * -1 : 0m,
                            ApplicationUserName = line.JournalApplicationUserName,
                            EnvironmentUserName = line.JournalEnvironmentUserName,
                            EnvironmentMachineName = line.JournalEnvironmentMachineName,
                            EnvironmentDomainName = line.JournalEnvironmentDomainName,
                            EnvironmentOSVersion = line.JournalEnvironmentOSVersion,
                            EnvironmentMACAddress = line.JournalEnvironmentMACAddress,
                            EnvironmentMotherboardSerialNumber = line.JournalEnvironmentMotherboardSerialNumber,
                            EnvironmentProcessorId = line.JournalEnvironmentProcessorId,
                            EnvironmentIPAddress = line.JournalEnvironmentIPAddress,
                        };

                    generalLedgerTransactions[index] = glTransaction;
                });

                // Step 2: Work out cumulative balance >> sequence matters, izmoto 25.04.2012!

                if (generalLedgerTransactions != null)
                {
                    await FlattenAsync(generalLedgerTransactions, serviceHeader);

                    generalLedgerTransactions = generalLedgerTransactions.Where(x => x != null).OrderBy(x => x.JournalCreatedDate).ToArray();

                    decimal cumulativeAvailableBalance = 0m;
                    decimal cumulativeBookBalance = 0m;

                    if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Savings))
                    {
                        cumulativeAvailableBalance += tuple.Item1;
                    }
                    else if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan, (int)ProductCode.Investment))
                    {
                        cumulativeBookBalance += tuple.Item2;
                    }

                    Array.ForEach(generalLedgerTransactions, (line) =>
                    {
                        var netAvailableValue = 0m;

                        var netBookValue = 0m;

                        switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                        {
                            case ProductCode.Savings:

                                switch ((CustomerAccountStatementType)customerAccountDTO.CustomerAccountStatementType)
                                {
                                    case CustomerAccountStatementType.FixedDepositStatement:
                                    case CustomerAccountStatementType.ChequeDepositStatement:

                                        netBookValue = (line.Credit + (line.Debit * -1));

                                        break;
                                    case CustomerAccountStatementType.PrincipalStatement:
                                    default:

                                        netAvailableValue = (line.Credit + (line.Debit * -1));

                                        break;
                                }

                                break;
                            case ProductCode.Loan:
                            case ProductCode.Investment:

                                netBookValue = (line.Credit + (line.Debit * -1));

                                break;
                            default:
                                break;
                        }

                        cumulativeAvailableBalance += netAvailableValue;
                        cumulativeBookBalance += netBookValue;

                        line.AvailableBalance = cumulativeAvailableBalance;
                        line.BookBalance = cumulativeBookBalance;

                        if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Savings))
                        {
                            if (customerAccountDTO.CustomerAccountStatementType.In((int)CustomerAccountStatementType.ChequeDepositStatement, (int)CustomerAccountStatementType.FixedDepositStatement))
                            {
                                line.RunningBalance = line.BookBalance;
                            }
                            else line.RunningBalance = line.AvailableBalance;
                        }
                        else if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan, (int)ProductCode.Investment))
                        {
                            line.RunningBalance = line.BookBalance;
                        }
                    });

                    result.AddRange(generalLedgerTransactions);
                }
            }

            return result;
        }

        private void Flatten(GeneralLedgerTransaction[] generalLedgerTransactions, ServiceHeader serviceHeader)
        {
            if (generalLedgerTransactions != null && generalLedgerTransactions.Any())
            {
                var chartOfAccountIds = generalLedgerTransactions.Select(x => x.GLAccountId);

                var contraChartOfAccountIds = generalLedgerTransactions.Select(x => x.ContraGLAccountId);

                var chartOfAccountIdsUnion = chartOfAccountIds.Union(contraChartOfAccountIds);

                var customerAccountIds = generalLedgerTransactions.Where(x => x.CustomerAccountId.HasValue).Select(x => x.CustomerAccountId.Value);

                var chartOfAccountSummaryDTOs = _chartOfAccountAppService.FindChartOfAccounts(chartOfAccountIdsUnion.ToArray(), serviceHeader);

                var customerAccountSummaryDTOS = _customerAccountAppService.FindCustomerAccounts(customerAccountIds.ToArray(), serviceHeader);

                foreach (var item in generalLedgerTransactions)
                {
                    var chartOfAccountSummaryDTO = chartOfAccountSummaryDTOs.SingleOrDefault(x => x.Id == item.GLAccountId);

                    var contraChartOfAccountSummaryDTO = chartOfAccountSummaryDTOs.SingleOrDefault(x => x.Id == item.ContraGLAccountId);

                    if (chartOfAccountSummaryDTO != null)
                    {
                        item.GLAccountCode = chartOfAccountSummaryDTO.AccountCode;
                        item.GLAccountType = chartOfAccountSummaryDTO.AccountType;
                        item.GLAccountDescription = chartOfAccountSummaryDTO.AccountName;
                    }

                    if (contraChartOfAccountSummaryDTO != null)
                    {
                        item.ContraGLAccountCode = contraChartOfAccountSummaryDTO.AccountCode;
                        item.ContraGLAccountType = contraChartOfAccountSummaryDTO.AccountType;
                        item.ContraGLAccountDescription = contraChartOfAccountSummaryDTO.AccountName;
                    }

                    if (item.CustomerAccountId.HasValue)
                    {
                        var customerAccountSummaryDTO = customerAccountSummaryDTOS.SingleOrDefault(x => x.Id == item.CustomerAccountId.Value);

                        if (customerAccountSummaryDTO != null)
                        {
                            item.CustomerAccountNumber = customerAccountSummaryDTO.FullAccountNumber;
                            item.CustomerFullName = customerAccountSummaryDTO.CustomerFullName;
                            item.CustomerReference1 = customerAccountSummaryDTO.CustomerReference1;
                            item.CustomerReference2 = customerAccountSummaryDTO.CustomerReference2;
                            item.CustomerReference2 = customerAccountSummaryDTO.CustomerReference3;
                        }
                    }
                }
            }
        }

        private async Task FlattenAsync(GeneralLedgerTransaction[] generalLedgerTransactions, ServiceHeader serviceHeader)
        {
            if (generalLedgerTransactions != null && generalLedgerTransactions.Any())
            {
                var chartOfAccountIds = generalLedgerTransactions.Select(x => x.GLAccountId);

                var contraChartOfAccountIds = generalLedgerTransactions.Select(x => x.ContraGLAccountId);

                var chartOfAccountIdsUnion = chartOfAccountIds.Union(contraChartOfAccountIds);

                var customerAccountIds = generalLedgerTransactions.Where(x => x.CustomerAccountId.HasValue).Select(x => x.CustomerAccountId.Value);

                var chartOfAccountSummaryDTOs = await _chartOfAccountAppService.FindChartOfAccountsAsync(chartOfAccountIdsUnion.ToArray(), serviceHeader);

                var customerAccountSummaryDTOS = await _customerAccountAppService.FindCustomerAccountsAsync(customerAccountIds.ToArray(), serviceHeader);

                foreach (var item in generalLedgerTransactions)
                {
                    var chartOfAccountSummaryDTO = chartOfAccountSummaryDTOs.SingleOrDefault(x => x.Id == item.GLAccountId);

                    var contraChartOfAccountSummaryDTO = chartOfAccountSummaryDTOs.SingleOrDefault(x => x.Id == item.ContraGLAccountId);

                    if (chartOfAccountSummaryDTO != null)
                    {
                        item.GLAccountCode = chartOfAccountSummaryDTO.AccountCode;
                        item.GLAccountType = chartOfAccountSummaryDTO.AccountType;
                        item.GLAccountDescription = chartOfAccountSummaryDTO.AccountName;
                    }

                    if (contraChartOfAccountSummaryDTO != null)
                    {
                        item.ContraGLAccountCode = contraChartOfAccountSummaryDTO.AccountCode;
                        item.ContraGLAccountType = contraChartOfAccountSummaryDTO.AccountType;
                        item.ContraGLAccountDescription = contraChartOfAccountSummaryDTO.AccountName;
                    }

                    if (item.CustomerAccountId.HasValue)
                    {
                        var customerAccountSummaryDTO = customerAccountSummaryDTOS.SingleOrDefault(x => x.Id == item.CustomerAccountId.Value);

                        if (customerAccountSummaryDTO != null)
                        {
                            item.CustomerAccountNumber = customerAccountSummaryDTO.FullAccountNumber;
                            item.CustomerFullName = customerAccountSummaryDTO.CustomerFullName;
                            item.CustomerReference1 = customerAccountSummaryDTO.CustomerReference1;
                            item.CustomerReference2 = customerAccountSummaryDTO.CustomerReference2;
                            item.CustomerReference2 = customerAccountSummaryDTO.CustomerReference3;
                        }
                    }
                }
            }
        }
    }
}
