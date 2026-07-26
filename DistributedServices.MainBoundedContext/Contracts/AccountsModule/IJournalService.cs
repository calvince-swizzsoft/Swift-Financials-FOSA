using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IJournalService
    {
        #region Journal

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO AddJournal(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, List<TariffWrapper> tariffs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO AddJournalSingleEntry(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid chartOfAccountId, Guid contraChartOfAccountId, int journalType, List<TariffWrapper> tariffs);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO AddJournalWithApportionments(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<ApportionmentWrapper> apportionments, List<TariffWrapper> tariffs, List<DynamicChargeDTO> dynamicCharges);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO AddCashManagementJournal(FiscalCountDTO fiscalCountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO AddJournalWithCustomerAccount(Guid branchId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO AddJournalWithCustomerAccountAndTariffs(Guid branchId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO AddJournalWithCustomerAccountAndAlternateChannelLogAndTariffs(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddTariffJournalsWithCustomerAccount(Guid? parentJournalId, Guid branchId, Guid alternateChannelLogId, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalDTO> FindReversibleJournalsByDateRangeAndFilterInPage(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ReverseJournals(List<JournalDTO> journalDTOs, string description, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ReverseAlternateChannelJournals(Guid[] alternateChannelLogIds);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO FindJournal(Guid journalId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<JournalEntryDTO> FindJournalEntriesByJournalId(Guid journalId);

        #endregion

        #region Financials

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double FV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double PV = 0, int Due = 0);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double PV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double FV = 0, int Due = 0);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double Pmt(int interestCalculationMode, int termInMonths, int paymentFrequencyPerYear, double APR, double PV, double FV = 0, int Due = 0);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double PPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double IPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double NPer(int paymentFrequencyPerYear, double APR, double Pmt, double PV, double FV = 0, int Due = 0);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AmortizationTableEntry> RepaymentSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, int interestCalculationMode, double APR, double PV, double FV = 0, int Due = 0);

        #endregion
    }
}
