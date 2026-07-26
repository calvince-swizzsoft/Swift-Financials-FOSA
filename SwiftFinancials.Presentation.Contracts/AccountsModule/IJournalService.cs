using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IJournalService")]
    public interface IJournalService
    {
        #region Journal

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournal(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, List<TariffWrapper> tariffs, AsyncCallback callback, Object state);
        JournalDTO EndAddJournal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalSingleEntry(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid chartOfAccountId, Guid contraChartOfAccountId, int journalType, List<TariffWrapper> tariffs, AsyncCallback callback, Object state);
        JournalDTO EndAddJournalSingleEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalWithApportionments(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<ApportionmentWrapper> apportionments, List<TariffWrapper> tariffs, List<DynamicChargeDTO> dynamicCharges, AsyncCallback callback, Object state);
        JournalDTO EndAddJournalWithApportionments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCashManagementJournal(FiscalCountDTO fiscalCountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, AsyncCallback callback, Object state);
        JournalDTO EndAddCashManagementJournal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalWithCustomerAccount(Guid branchId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, AsyncCallback callback, Object state);
        JournalDTO EndAddJournalWithCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalWithCustomerAccountAndTariffs(Guid branchId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs, AsyncCallback callback, Object state);
        JournalDTO EndAddJournalWithCustomerAccountAndTariffs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalWithCustomerAccountAndAlternateChannelLogAndTariffs(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs, AsyncCallback callback, Object state);
        JournalDTO EndAddJournalWithCustomerAccountAndAlternateChannelLogAndTariffs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddTariffJournalsWithCustomerAccount(Guid? parentJournalId, Guid branchId, Guid alternateChannelLogId, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs, AsyncCallback callback, Object state);
        bool EndAddTariffJournalsWithCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReversibleJournalsByDateRangeAndFilterInPage(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalDTO> EndFindReversibleJournalsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginReverseJournals(List<JournalDTO> journalDTOs, string description, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndReverseJournals(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginReverseAlternateChannelJournals(Guid[] alternateChannelLogIds, AsyncCallback callback, Object state);
        bool EndReverseAlternateChannelJournals(IAsyncResult result);
        
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournal(Guid journalId, AsyncCallback callback, Object state);
        JournalDTO EndFindJournal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalEntriesByJournalId(Guid journalId, AsyncCallback callback, Object state);
        List<JournalEntryDTO> EndFindJournalEntriesByJournalId(IAsyncResult result);

        #endregion

        #region Financials

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double PV, int Due, AsyncCallback callback, Object state);
        double EndFV(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double FV, int Due, AsyncCallback callback, Object state);
        double EndPV(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPmt(int interestCalculationMode, int termInMonths, int paymentFrequencyPerYear, double APR, double PV, double FV, int Due, AsyncCallback callback, Object state);
        double EndPmt(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV, int Due, AsyncCallback callback, Object state);
        double EndPPmt(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV, int Due, AsyncCallback callback, Object state);
        double EndIPmt(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginNPer(int paymentFrequencyPerYear, double APR, double Pmt, double PV, double FV, int Due, AsyncCallback callback, Object state);
        double EndNPer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRepaymentSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, int interestCalculationMode, double APR, double PV, double FV, int Due, AsyncCallback callback, Object state);
        List<AmortizationTableEntry> EndRepaymentSchedule(IAsyncResult result);

        #endregion
    }
}
