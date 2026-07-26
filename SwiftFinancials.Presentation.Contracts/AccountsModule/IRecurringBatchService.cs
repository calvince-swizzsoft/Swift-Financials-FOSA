using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IRecurringBatchService")]
    public interface IRecurringBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindRecurringBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<RecurringBatchDTO> EndFindRecurringBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindRecurringBatchEntriesByRecurringBatchIdInPage(Guid recurringBatchId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<RecurringBatchEntryDTO> EndFindRecurringBatchEntriesByRecurringBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueableRecurringBatchEntriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<RecurringBatchEntryDTO> EndFindQueableRecurringBatchEntriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCapitalizeInterestByEmployersAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<LoanProductDTO> loanProductDTOs, AsyncCallback callback, Object state);
        bool EndCapitalizeInterestByEmployersAndLoanProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCapitalizeInterestByCustomersAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<LoanProductDTO> loanProductDTOs, AsyncCallback callback, Object state);
        bool EndCapitalizeInterestByCustomersAndLoanProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCapitalizeInterestByCreditTypesAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<LoanProductDTO> loanProductDTOs, AsyncCallback callback, Object state);
        bool EndCapitalizeInterestByCreditTypesAndLoanProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCapitalizeInterest(int priority, AsyncCallback callback, Object state);
        bool EndCapitalizeInterest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginChargeLoanDynamicFees(RecurringBatchDTO recurringBatchDTO, List<LoanProductDTO> loanProductDTOs, AsyncCallback callback, Object state);
        bool EndChargeLoanDynamicFees(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginChargeSavingsDynamicFees(RecurringBatchDTO recurringBatchDTO, List<SavingsProductDTO> savingsProductDTOs, AsyncCallback callback, Object state);
        bool EndChargeSavingsDynamicFees(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<StandingOrderDTO> standingOrderDTOs, AsyncCallback callback, Object state);
        bool EndExecuteStandingOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteStandingOrdersByBenefactorEmployerAndTrigger(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, int standingOrderTrigger, AsyncCallback callback, Object state);
        bool EndExecuteStandingOrdersByBenefactorEmployerAndTrigger(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostRecurringBatchEntry(Guid recurringBatchEntryId, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPostRecurringBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginNormalizeInvestmentBalances(string investmentNormalizationSets, int priority, bool enforceCeiling, AsyncCallback callback, Object state);
        bool EndNormalizeInvestmentBalances(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteScheduledStandingOrders(DateTime targetDate, int targetDateOption, int priority, int maximumStandingOrderExecuteAttemptCount, int pageSize, AsyncCallback callback, Object state);
        bool EndExecuteScheduledStandingOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteStandingOrdersByBenefactorEmployers(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, AsyncCallback callback, Object state);
        bool EndExecuteStandingOrdersByBenefactorEmployers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteStandingOrdersByBenefactorCustomers(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, AsyncCallback callback, Object state);
        bool EndExecuteStandingOrdersByBenefactorCustomers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteStandingOrdersByBenefactorCreditTypes(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, AsyncCallback callback, Object state);
        bool EndExecuteStandingOrdersByBenefactorCreditTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteSweepingStandingOrders(int priority, int pageSize, AsyncCallback callback, Object state);
        bool EndExecuteSweepingStandingOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPoolInvestmentBalances(int priority, AsyncCallback callback, Object state);
        bool EndPoolInvestmentBalances(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginReleaseLoanGuarantors(int priority, AsyncCallback callback, Object state);
        bool EndReleaseLoanGuarantors(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecuteElectronicStatementOrders(DateTime targetDate, int targetDateOption, string sender, int priority, int pageSize, AsyncCallback callback, Object state);
        bool EndExecuteElectronicStatementOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRecoverArrears(int priority, int pageSize, AsyncCallback callback, Object state);
        bool EndRecoverArrears(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRecoverArrearsFromInvestmentProduct(int priority, string targetProductCodes, int pageSize, AsyncCallback callback, Object state);
        bool EndRecoverArrearsFromInvestmentProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginProcessSavingsProductLedgerFees(int priority, AsyncCallback callback, Object state);
        bool EndProcessSavingsProductLedgerFees(IAsyncResult result);
    }
}
