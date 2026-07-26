using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IRecurringBatchAppService
    {
        RecurringBatchDTO AddNewRecurringBatch(RecurringBatchDTO recurringBatchDTO, ServiceHeader serviceHeader);

        bool UpdateRecurringBatchEntries(Guid recurringBatchId, List<RecurringBatchEntryDTO> recurringBatchEntries, ServiceHeader serviceHeader);

        bool PostRecurringBatchEntry(Guid recurringBatchEntryId, int moduleNavigationItemCode, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader);

        List<RecurringBatchDTO> FindRecurringBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<RecurringBatchDTO> FindRecurringBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        RecurringBatchDTO FindRecurringBatch(Guid recurringBatchId, ServiceHeader serviceHeader);

        List<RecurringBatchEntryDTO> FindRecurringBatchEntryDTOsByRecurringBatchId(Guid recurringBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<RecurringBatchEntryDTO> FindRecurringBatchEntriesByRecurringBatchId(Guid recurringBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<RecurringBatchEntryDTO> FindQueableRecurringBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool CapitalizeInterest(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader);

        bool CapitalizeInterest(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader);

        bool CapitalizeInterest(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader);

        bool CapitalizeInterest(int priority, ServiceHeader serviceHeader);

        bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<StandingOrderDTO> standingOrderDTOs, ServiceHeader serviceHeader);

        bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, int standingOrderTrigger, ServiceHeader serviceHeader);

        bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader);

        bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader);

        bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader);

        bool ExecuteStandingOrders(DateTime targetDate, int targetDateOption, int priority, int maximumStandingOrderExecuteAttemptCount, int pageSize, ServiceHeader serviceHeader);

        bool ExecutePayoutStandingOrders(Guid benefactorCustomerAccountId, int month, int priority, ServiceHeader serviceHeader);

        bool ExecuteSweepingStandingOrders(int priority, int pageSize, ServiceHeader serviceHeader);

        bool ChargeDynamicFees(RecurringBatchDTO recurringBatchDTO, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader);

        bool ChargeDynamicFees(RecurringBatchDTO recurringBatchDTO, List<SavingsProductDTO> savingsProductDTOs, ServiceHeader serviceHeader);

        bool AdjustInvestmentBalances(string investmentNormalizationSets, int priority, bool enforceCeiling, ServiceHeader serviceHeader);

        bool PoolInvestmentBalances(int priority, ServiceHeader serviceHeader);

        bool ReleaseLoanGuarantors(int priority, ServiceHeader serviceHeader);

        bool ExecuteElectronicStatementOrders(DateTime targetDate, int targetDateOption, string sender, int priority, int pageSize, ServiceHeader serviceHeader);

        bool RecoverArrears(int priority, int pageSize, ServiceHeader serviceHeader);

        bool RecoverArrearsFromInvestmentProduct(int priority, string targetProductCodes, int pageSize, ServiceHeader serviceHeader);

        bool RecoverArrears(List<StandingOrderDTO> standingOrderDTOs, int priority, ServiceHeader serviceHeader);

        bool RecoverArrears(ExternalChequeDTO externalChequeDTO, List<ExternalChequePayableDTO> externalChequePayables, int priority, ServiceHeader serviceHeader);

        bool RecoverArrears(FixedDepositDTO fixedDepositDTO, List<FixedDepositPayableDTO> fixedDepositPayables, int priority, ServiceHeader serviceHeader);

        bool RecoverArrears(CreditBatchEntryDTO creditBatchEntryDTO, List<LoanProductDTO> loanProductCollection, int priority, ServiceHeader serviceHeader);

        bool RecoverArrears(CustomerAccountDTO benefactorCustomerAccount, int priority, ServiceHeader serviceHeader);

        bool ProcessSavingsProductLedgerFees(int priority, ServiceHeader serviceHeader);
    }
}
