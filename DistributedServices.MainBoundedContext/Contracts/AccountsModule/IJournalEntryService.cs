using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IJournalEntryService
    {
        #region Journal Entry

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindLastXGeneralLedgerTransactionsByCustomerAccountIdAsync(CustomerAccountDTO customerAccountDTO, int lastXItems, bool tallyDebitsCredits);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool tallyDebitsCredits);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<JournalEntryDTO>> FindReversibleJournalEntriesByDateRangeAndFilterInPageAsync(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int pageIndex, int pageSize);

        #endregion
    }
}
