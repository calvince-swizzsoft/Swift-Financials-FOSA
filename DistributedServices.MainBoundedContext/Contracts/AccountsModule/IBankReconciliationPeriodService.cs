using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IBankReconciliationPeriodService
    {
        #region Bank Reconciliation Period

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankReconciliationPeriodDTO AddBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CloseBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, int bankReconciliationPeriodAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BankReconciliationPeriodDTO> FindBankReconciliationPeriods();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriodsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankReconciliationPeriodDTO FindBankReconciliationPeriod(Guid bankReconciliationPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankReconciliationEntryDTO AddBankReconciliationEntry(BankReconciliationEntryDTO bankReconciliationEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveBankReconciliationEntries(List<BankReconciliationEntryDTO> bankReconciliationEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankReconciliationEntryDTO> FindBankReconciliationEntriesByBankReconciliationPeriodIdAndFilterInPage(Guid bankReconciliationPeriodId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriodsByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
