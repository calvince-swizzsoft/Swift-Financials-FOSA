using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class RecurringBatchService : IRecurringBatchService
    {
        private readonly IRecurringBatchAppService _recurringBatchAppService;

        public RecurringBatchService(
           IRecurringBatchAppService recurringBatchAppService)
        {
            Guard.ArgumentNotNull(recurringBatchAppService, nameof(recurringBatchAppService));

            _recurringBatchAppService = recurringBatchAppService;
        }

        public bool CapitalizeInterestByEmployersAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<LoanProductDTO> loanProductDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.CapitalizeInterest(recurringBatchDTO, employerDTOs, loanProductDTOs, serviceHeader);
        }

        public bool CapitalizeInterestByCustomersAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<LoanProductDTO> loanProductDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.CapitalizeInterest(recurringBatchDTO, customerDTOs, loanProductDTOs, serviceHeader);
        }

        public bool CapitalizeInterestByCreditTypesAndLoanProducts(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<LoanProductDTO> loanProductDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.CapitalizeInterest(recurringBatchDTO, creditTypeDTOs, loanProductDTOs, serviceHeader);
        }

        public bool CapitalizeInterest(int priority)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.CapitalizeInterest(priority, serviceHeader);
        }

        public bool ChargeLoanDynamicFees(RecurringBatchDTO recurringBatchDTO, List<LoanProductDTO> loanProductDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ChargeDynamicFees(recurringBatchDTO, loanProductDTOs, serviceHeader);
        }

        public bool ChargeSavingsDynamicFees(RecurringBatchDTO recurringBatchDTO, List<SavingsProductDTO> savingsProductDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ChargeDynamicFees(recurringBatchDTO, savingsProductDTOs, serviceHeader);
        }

        public bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<StandingOrderDTO> standingOrderDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteStandingOrders(recurringBatchDTO, standingOrderDTOs, serviceHeader);
        }

        public bool ExecuteStandingOrdersByBenefactorEmployerAndTrigger(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, int standingOrderTrigger)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteStandingOrders(recurringBatchDTO, employerDTOs, standingOrderTrigger, serviceHeader);
        }

        public bool ExecuteStandingOrdersByBenefactorEmployers(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteStandingOrders(recurringBatchDTO, employerDTOs, savingsProductDTOs, loanProductDTOs, investmentProductDTOs, standingOrderTrigger, serviceHeader);
        }

        public bool ExecuteStandingOrdersByBenefactorCustomers(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteStandingOrders(recurringBatchDTO, customerDTOs, savingsProductDTOs, loanProductDTOs, investmentProductDTOs, standingOrderTrigger, serviceHeader);
        }

        public bool ExecuteStandingOrdersByBenefactorCreditTypes(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteStandingOrders(recurringBatchDTO, creditTypeDTOs, savingsProductDTOs, loanProductDTOs, investmentProductDTOs, standingOrderTrigger, serviceHeader);
        }

        public bool PostRecurringBatchEntry(Guid recurringBatchEntryId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _recurringBatchAppService.PostRecurringBatchEntry(recurringBatchEntryId, moduleNavigationItemCode, serviceBrokerSettingsElement.FileExportDirectory, ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString, serviceHeader);
        }

        public List<RecurringBatchDTO> FindRecurringBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.FindRecurringBatches(serviceHeader);
        }

        public PageCollectionInfo<RecurringBatchDTO> FindRecurringBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.FindRecurringBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public RecurringBatchDTO FindRecurringBatch(Guid recurringBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.FindRecurringBatch(recurringBatchId, serviceHeader);
        }

        public List<RecurringBatchEntryDTO> FindRecurringBatchEntriesByRecurringBatchId(Guid recurringBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.FindRecurringBatchEntryDTOsByRecurringBatchId(recurringBatchId, serviceHeader);
        }

        public PageCollectionInfo<RecurringBatchEntryDTO> FindRecurringBatchEntriesByRecurringBatchIdInPage(Guid recurringBatchId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.FindRecurringBatchEntriesByRecurringBatchId(recurringBatchId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<RecurringBatchEntryDTO> FindQueableRecurringBatchEntriesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.FindQueableRecurringBatchEntries(pageIndex, pageSize, serviceHeader);
        }

        public bool ExecuteScheduledStandingOrders(DateTime targetDate, int targetDateOption, int priority, int maximumStandingOrderExecuteAttemptCount, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteStandingOrders(targetDate, targetDateOption, priority, maximumStandingOrderExecuteAttemptCount, pageSize, serviceHeader);
        }

        public bool ExecuteSweepingStandingOrders(int priority, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteSweepingStandingOrders(priority, pageSize, serviceHeader);
        }

        public bool NormalizeInvestmentBalances(string investmentNormalizationSets, int priority, bool enforceCeiling)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.AdjustInvestmentBalances(investmentNormalizationSets, priority, enforceCeiling, serviceHeader);
        }

        public bool PoolInvestmentBalances(int priority)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.PoolInvestmentBalances(priority, serviceHeader);
        }

        public bool ReleaseLoanGuarantors(int priority)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ReleaseLoanGuarantors(priority, serviceHeader);
        }

        public bool ExecuteElectronicStatementOrders(DateTime targetDate, int targetDateOption, string sender, int priority, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ExecuteElectronicStatementOrders(targetDate, targetDateOption, sender, priority, pageSize, serviceHeader);
        }

        public bool RecoverArrears(int priority, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.RecoverArrears(priority, pageSize, serviceHeader);
        }

        public bool RecoverArrearsFromInvestmentProduct(int priority, string targetProductCodes, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.RecoverArrearsFromInvestmentProduct(priority, targetProductCodes, pageSize, serviceHeader);
        }

        public bool ProcessSavingsProductLedgerFees(int priority)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _recurringBatchAppService.ProcessSavingsProductLedgerFees(priority, serviceHeader);
        }
    }
}
