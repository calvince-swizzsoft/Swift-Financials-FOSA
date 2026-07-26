using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IJournalEntryAppService
    {
        PageCollectionInfo<GeneralLedgerTransaction> FindLastXGeneralLedgerTransactionsByCustomerAccountId(CustomerAccountDTO customerAccountDTO, int lastXDays, int lastXItems, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindLastXGeneralLedgerTransactionsByCustomerAccountIdAsync(CustomerAccountDTO customerAccountDTO, int lastXDays, int lastXItems, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalEntryDTO> FindReversibleJournalEntriesInPage(int pageIndex, int pageSize, int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<JournalEntryDTO>> FindReversibleJournalEntriesInPageAsync(int pageIndex, int pageSize, int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsInPage(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeInPage(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPage(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerTransaction> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPage(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, ServiceHeader serviceHeader);
    }
}
