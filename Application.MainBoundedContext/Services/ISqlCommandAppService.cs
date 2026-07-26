using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.Services
{
    public interface ISqlCommandAppService
    {
        List<CustomerDTO> FindCustomersByPayrollNumber(string payrollNumber, ServiceHeader serviceHeader);

        Task<List<CustomerDTO>> FindCustomersByPayrollNumberAsync(string payrollNumber, ServiceHeader serviceHeader);

        CustomerAccountDTO FindCustomerAccountById(Guid customerAccountId, ServiceHeader serviceHeader);

        Task<CustomerAccountDTO> FindCustomerAccountByIdAsync(Guid customerAccountId, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByTargetProductIdAndReference3(Guid targetProductId, string reference3, ServiceHeader serviceHeader);

        Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAndReference3Async(Guid targetProductId, string reference3, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByTargetProductIdAndPayrollNumber(Guid targetProductId, string payrollNumber, ServiceHeader serviceHeader);

        Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAndPayrollNumberAsync(Guid targetProductId, string payrollNumber, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByTargetProductId(Guid targetProductId, ServiceHeader serviceHeader);

        Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAsync(Guid targetProductId, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsGivenEmployerAndLoanProduct(Guid employerId, Guid loanProductId, ServiceHeader serviceHeader);

        Task<List<CustomerAccountDTO>> FindCustomerAccountsGivenEmployerAndLoanProductAsync(Guid employerId, Guid loanProductId, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsGivenCreditTypeAndLoanProduct(Guid creditTypeId, Guid loanProductId, ServiceHeader serviceHeader);

        Task<List<CustomerAccountDTO>> FindCustomerAccountsGivenCreditTypeAndLoanProductAsync(Guid creditTypeId, Guid loanProductId, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByTargetProductIdAndCustomerId(Guid targetProductId, Guid customerId, ServiceHeader serviceHeader);

        Task<List<CustomerAccountDTO>> FindCustomerAccountsByTargetProductIdAndCustomerIdAsync(Guid targetProductId, Guid customerId, ServiceHeader serviceHeader);

        int DeleteDebitBatchEntries(Guid debitBatchId, ServiceHeader serviceHeader);

        Task<int> DeleteDebitBatchEntriesAsync(Guid debitBatchId, ServiceHeader serviceHeader);

        decimal FindCreditBatchEntriesTotal(Guid creditBatchId, ServiceHeader serviceHeader);

        Task<decimal> FindCreditBatchEntriesTotalAsync(Guid creditBatchId, ServiceHeader serviceHeader);

        int DeleteCreditBatchEntries(Guid creditBatchId, ServiceHeader serviceHeader);

        int DeleteOverDeductionBatchEntries(Guid overDeductionBatchId, ServiceHeader serviceHeader);

        Task<int> DeleteCreditBatchEntriesAsync(Guid creditBatchId, ServiceHeader serviceHeader);

        decimal FindWireTransferBatchEntriesTotal(Guid wireTransferBatchId, ServiceHeader serviceHeader);

        Task<decimal> FindWireTransferBatchEntriesTotalAsync(Guid wireTransferBatchId, ServiceHeader serviceHeader);

        int DeleteWireTransferBatchEntries(Guid wireTransferBatchId, ServiceHeader serviceHeader);

        Task<int> DeleteWireTransferBatchEntriesAsync(Guid wireTransferBatchId, ServiceHeader serviceHeader);

        int DeleteAlternateChannelReconciliationEntries(Guid alternateChannelReconciliationPeriodId, ServiceHeader serviceHeader);

        Task<int> DeleteAlternateChannelReconciliationEntriesAsync(Guid alternateChannelReconciliationPeriodId, ServiceHeader serviceHeader);

        int DeleteCreditBatchDiscrepancies(Guid creditBatchId, ServiceHeader serviceHeader);

        int DeleteOverDeductionBatchDiscrepancies(Guid overDeductionBatchId, ServiceHeader serviceHeader);

        Task<int> DeleteCreditBatchDiscrepanciesAsync(Guid creditBatchId, ServiceHeader serviceHeader);

        int DeleteLoanDisbursementBatchEntries(Guid loanDisbursementBatchId, ServiceHeader serviceHeader);

        Task<int> DeleteLoanDisbursementBatchEntriesAsync(Guid loanDisbursementBatchId, ServiceHeader serviceHeader);

        int DeleteJournalReversalBatchEntries(Guid journalReversalBatchId, ServiceHeader serviceHeader);

        Task<int> DeleteJournalReversalBatchEntriesAsync(Guid journalReversalBatchId, ServiceHeader serviceHeader);

        int CheckCapitalization(Guid customerAccountId, int month, int interestCapitalizationMonths, ServiceHeader serviceHeader);

        Task<int> CheckCapitalizationAsync(Guid customerAccountId, int month, int interestCapitalizationMonths, ServiceHeader serviceHeader);

        decimal FindCustomerAccountBookBalance(CustomerAccountDTO customerAccountDTO, int type, DateTime cutOffDate, ServiceHeader serviceHeader, bool considerMaturityPeriodForInvestmentAccounts = false);

        Task<decimal> FindCustomerAccountBookBalanceAsync(CustomerAccountDTO customerAccountDTO, int type, DateTime cutOffDate, ServiceHeader serviceHeader, bool considerMaturityPeriodForInvestmentAccounts = false);

        decimal FindCustomerAccountAvailableBalance(CustomerAccountDTO customerAccountDTO, DateTime cutOffDate, ServiceHeader serviceHeader);

        Task<decimal> FindCustomerAccountAvailableBalanceAsync(CustomerAccountDTO customerAccountDTO, DateTime cutOffDate, ServiceHeader serviceHeader);

        decimal FindGlAccountBalance(Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader);

        Task<decimal> FindGlAccountBalanceAsync(Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader);

        decimal FindGlAccountBalance(Guid branchId, Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader);

        decimal FindGlAccountBalance(Guid branchId, Guid chartOfAccountId, Guid postingPeriodId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader);

        Task<decimal> FindGlAccountBalanceAsync(Guid branchId, Guid chartOfAccountId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader);

        decimal FindDisbursedLoanCasesValue(Guid loanProductId, Guid branchId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader);

        decimal ComputeDividendsPayable(Guid customerId, ServiceHeader serviceHeader);

        Task<decimal> ComputeDividendsPayableAsync(Guid customerId, ServiceHeader serviceHeader);

        //Tuple<decimal, decimal, int> FindGlAccountStatistics(Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionDateFilter, ServiceHeader serviceHeader);

        Tuple<decimal, decimal, decimal, decimal, int> FindGlAccountStatistics(Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionDateFilter, ServiceHeader serviceHeader);

        Task<Tuple<decimal, decimal, int>> FindGlAccountStatisticsAsync(Guid chartOfAccountId, DateTime startDate, DateTime endDate, int transactionDateFilter, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByEmployerAndTrigger(Guid employerId, int trigger, ServiceHeader serviceHeader);

        Task<List<StandingOrderDTO>> FindStandingOrdersByEmployerAndTriggerAsync(Guid employerId, int trigger, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByEmployerAndProductAndTrigger(Guid employerId, Guid productId, int trigger, ServiceHeader serviceHeader);

        Task<List<StandingOrderDTO>> FindStandingOrdersByEmployerAndProductAndTriggerAsync(Guid employerId, Guid productId, int trigger, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByCreditTypeAndProductAndTrigger(Guid creditTypeId, Guid productId, int trigger, ServiceHeader serviceHeader);

        Task<List<StandingOrderDTO>> FindStandingOrdersByCreditTypeAndProductAndTriggerAsync(Guid creditTypeId, Guid productId, int trigger, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByCustomerAndProductAndTrigger(Guid customerId, Guid productId, int trigger, ServiceHeader serviceHeader);

        Task<List<StandingOrderDTO>> FindStandingOrdersByCustomerAndProductAndTriggerAsync(Guid customerId, Guid productId, int trigger, ServiceHeader serviceHeader);

        List<StandingOrderDTO> FindStandingOrdersByCustomerRerence3AndTrigger(string customerReference3, int trigger, ServiceHeader serviceHeader);

        Task<List<StandingOrderDTO>> FindStandingOrdersByCustomerRerence3AndTriggerAsync(string customerReference3, int trigger, ServiceHeader serviceHeader);

        StandingOrderDTO FindStandingOrder(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader);

        Task<StandingOrderDTO> FindStandingOrderAsync(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader);

        CreditBatchEntryDTO FindLastCreditBatchEntryByCustomerAccountId(Guid customerAccountId, int creditBatchType, ServiceHeader serviceHeader);

        Task<CreditBatchEntryDTO> FindLastCreditBatchEntryByCustomerAccountIdAsyn(Guid customerAccountId, int creditBatchType, ServiceHeader serviceHeader);

        LoanCaseDTO FindLastLoanCaseByCustomerId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);

        Task<LoanCaseDTO> FindLastLoanCaseByCustomerIdAsyn(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);

        List<Guid> FindCustomerIds(ServiceHeader serviceHeader);

        Task<List<Guid>> FindCustomerIdsAsync(ServiceHeader serviceHeader);

        int UpdateStandingOrderCapitalizedInterest(Guid beneficiaryCustomerAccountId, decimal interestAmount, ServiceHeader serviceHeader);

        Task<int> UpdateStandingOrderCapitalizedInterestAsync(Guid beneficiaryCustomerAccountId, decimal interestAmount, ServiceHeader serviceHeader);

        bool BulkInsert<T>(string tableName, IList<T> list, ServiceHeader serviceHeader);

        Task<bool> BulkInsertAsync<T>(string tableName, IList<T> list, ServiceHeader serviceHeader);

        DateTime? CheckCustomerAccountLastWithdrawalDate(Guid customerAccountId, ServiceHeader serviceHeader);

        Task<DateTime?> CheckCustomerAccountLastWithdrawalDateAsync(Guid customerAccountId, ServiceHeader serviceHeader);

        int DelinkStation(Guid stationId, ServiceHeader serviceHeader);

        Task<int> DelinkStationAsync(Guid stationId, ServiceHeader serviceHeader);

        SuperSaverInterestDTO FindCustomerSuperSaverPayable(Guid customerId, ServiceHeader serviceHeader);

        List<MonthlySummaryValuesDTO> FindEmailAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader);

        List<MonthlySummaryValuesDTO> FindTextAlertsMonthlyStatatistics(Guid companyId, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader);

        //  Add to the interface  
        JournalDTO FindLastPrincipalRepaymentByLoanReference(
    string loanCaseNumber,
    Guid customerAccountId,
    ServiceHeader serviceHeader);

        Task<JournalDTO> FindLastPrincipalRepaymentByLoanReferenceAsync(
            string loanCaseNumber,
            Guid customerAccountId,
            ServiceHeader serviceHeader);
    }
}
