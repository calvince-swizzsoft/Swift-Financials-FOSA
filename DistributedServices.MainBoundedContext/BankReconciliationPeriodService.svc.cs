using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BankReconciliationPeriodService : IBankReconciliationPeriodService
    {
        private readonly IBankReconciliationPeriodAppService _bankReconciliationPeriodAppService;

        public BankReconciliationPeriodService(
            IBankReconciliationPeriodAppService bankReconciliationPeriodAppService)
        {
            Guard.ArgumentNotNull(bankReconciliationPeriodAppService, nameof(bankReconciliationPeriodAppService));

            _bankReconciliationPeriodAppService = bankReconciliationPeriodAppService;
        }

        #region Bank Reconciliation Period

        public BankReconciliationPeriodDTO AddBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.AddNewBankReconciliationPeriod(bankReconciliationPeriodDTO, serviceHeader);
        }

        public bool UpdateBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.UpdateBankReconciliationPeriod(bankReconciliationPeriodDTO, serviceHeader);
        }

        public bool CloseBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, int bankReconciliationPeriodAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.CloseBankReconciliationPeriod(bankReconciliationPeriodDTO, bankReconciliationPeriodAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public List<BankReconciliationPeriodDTO> FindBankReconciliationPeriods()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.FindBankReconciliationPeriods(serviceHeader);
        }

        public PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriodsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.FindBankReconciliationPeriods(pageIndex, pageSize, serviceHeader);
        }

        public BankReconciliationPeriodDTO FindBankReconciliationPeriod(Guid bankReconciliationPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.FindBankReconciliationPeriod(bankReconciliationPeriodId, serviceHeader);
        }

        public BankReconciliationEntryDTO AddBankReconciliationEntry(BankReconciliationEntryDTO bankReconciliationEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.AddNewBankReconciliationEntry(bankReconciliationEntryDTO, serviceHeader);
        }

        public bool RemoveBankReconciliationEntries(List<BankReconciliationEntryDTO> bankReconciliationEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.RemoveBankReconciliationEntries(bankReconciliationEntryDTOs, serviceHeader);
        }

        public PageCollectionInfo<BankReconciliationEntryDTO> FindBankReconciliationEntriesByBankReconciliationPeriodIdAndFilterInPage(Guid bankReconciliationPeriodId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.FindBankReconciliationEntriesByBankReconciliationPeriodId(bankReconciliationPeriodId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriodsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankReconciliationPeriodAppService.FindBankReconciliationPeriods(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
