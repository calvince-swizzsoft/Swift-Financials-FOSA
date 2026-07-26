using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IJournalEntryService")]
    public interface IJournalEntryService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLastXGeneralLedgerTransactionsByCustomerAccountId(CustomerAccountDTO customerAccountDTO, int lastXItems, bool tallyDebitsCredits, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerTransaction> EndFindLastXGeneralLedgerTransactionsByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool tallyDebitsCredits, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerTransaction> EndFindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndFilterInPage(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerTransaction> EndFindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPage(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerTransaction> EndFindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPage(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerTransaction> EndFindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerTransactionsByDateRangeAndFilterInPage(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerTransaction> EndFindGeneralLedgerTransactionsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReversibleJournalEntriesByDateRangeAndFilterInPage(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalEntryDTO> EndFindReversibleJournalEntriesByDateRangeAndFilterInPage(IAsyncResult result);
    }
}
