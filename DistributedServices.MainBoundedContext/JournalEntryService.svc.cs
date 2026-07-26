using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class JournalEntryService : IJournalEntryService
    {
        private readonly IJournalEntryAppService _journalEntryAppService;

        public JournalEntryService(
            IJournalEntryAppService journalEntryAppService)
        {
            Guard.ArgumentNotNull(journalEntryAppService, nameof(journalEntryAppService));

            _journalEntryAppService = journalEntryAppService;
        }

        #region Journal Entry

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindLastXGeneralLedgerTransactionsByCustomerAccountIdAsync(CustomerAccountDTO customerAccountDTO, int lastXItems, bool tallyDebitsCredits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);
            
            return await _journalEntryAppService.FindLastXGeneralLedgerTransactionsByCustomerAccountIdAsync(customerAccountDTO, serviceBrokerSettingsElement.MiniStatementDaysCap, lastXItems, tallyDebitsCredits, serviceHeader);
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool tallyDebitsCredits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _journalEntryAppService.FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeAsync(customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, tallyDebitsCredits, serviceHeader);
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter, bool tallyDebitsCredits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _journalEntryAppService.FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeInPageAsync(pageIndex, pageSize, chartOfAccountId, startDate, endDate, text, journalEntryFilter, transactionDateFilter, tallyDebitsCredits, serviceHeader);
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPageAsync(int pageIndex, int pageSize, Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionCode, string reference, int transactionDateFilter, bool tallyDebitsCredits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _journalEntryAppService.FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPageAsync(pageIndex, pageSize, chartOfAccountId, startDate, endDate, transactionCode, reference, transactionDateFilter, tallyDebitsCredits, serviceHeader);
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(int pageIndex, int pageSize, CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, bool tallyDebitsCredits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _journalEntryAppService.FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPageAsync(pageIndex, pageSize, customerAccountDTO, startDate, endDate, text, journalEntryFilter, tallyDebitsCredits, serviceHeader);
        }

        public async Task<PageCollectionInfo<GeneralLedgerTransaction>> FindGeneralLedgerTransactionsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalEntryFilter)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _journalEntryAppService.FindGeneralLedgerTransactionsInPageAsync(pageIndex, pageSize, startDate, endDate, text, journalEntryFilter, serviceHeader);
        }

        public async Task<PageCollectionInfo<JournalEntryDTO>> FindReversibleJournalEntriesByDateRangeAndFilterInPageAsync(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _journalEntryAppService.FindReversibleJournalEntriesInPageAsync(pageIndex, pageSize, systemTransactionCode, startDate, endDate, text, journalEntryFilter, serviceHeader);
        }

        #endregion
    }
}
