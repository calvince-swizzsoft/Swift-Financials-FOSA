using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IRecurringBatchService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CapitalizeInterestByEmployersAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<LoanProductDTO> loanProductDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CapitalizeInterestByCustomersAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<LoanProductDTO> loanProductDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CapitalizeInterestByCreditTypesAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<LoanProductDTO> loanProductDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CapitalizeInterest(int priority);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ChargeLoanDynamicFees(RecurringBatchDTO recurringBatchDTO, List<LoanProductDTO> loanProductDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ChargeSavingsDynamicFees(RecurringBatchDTO recurringBatchDTO, List<SavingsProductDTO> savingsProductDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<StandingOrderDTO> standingOrderDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteStandingOrdersByBenefactorEmployerAndTrigger(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, int standingOrderTrigger);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteStandingOrdersByBenefactorEmployers(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteStandingOrdersByBenefactorCustomers(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteStandingOrdersByBenefactorCreditTypes(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostRecurringBatchEntry(Guid recurringBatchEntryId, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<RecurringBatchDTO> FindRecurringBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<RecurringBatchDTO> FindRecurringBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        RecurringBatchDTO FindRecurringBatch(Guid recurringBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<RecurringBatchEntryDTO> FindRecurringBatchEntriesByRecurringBatchId(Guid recurringBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<RecurringBatchEntryDTO> FindRecurringBatchEntriesByRecurringBatchIdInPage(Guid recurringBatchId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<RecurringBatchEntryDTO> FindQueableRecurringBatchEntriesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteScheduledStandingOrders(DateTime targetDate, int targetDateOption, int priority, int maximumStandingOrderExecuteAttemptCount, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteSweepingStandingOrders(int priority, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool NormalizeInvestmentBalances(string investmentNormalizationSets, int priority, bool enforceCeiling);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PoolInvestmentBalances(int priority);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ReleaseLoanGuarantors(int priority);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecuteElectronicStatementOrders(DateTime targetDate, int targetDateOption, string sender, int priority, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RecoverArrears(int priority, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RecoverArrearsFromInvestmentProduct(int priority, string targetProductCodes, int pageSize); 

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ProcessSavingsProductLedgerFees(int priority);
    }
}
