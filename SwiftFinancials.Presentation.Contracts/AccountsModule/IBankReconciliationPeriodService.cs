using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IBankReconciliationPeriodService")]
    public interface IBankReconciliationPeriodService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, AsyncCallback callback, Object state);
        BankReconciliationPeriodDTO EndAddBankReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, AsyncCallback callback, Object state);
        bool EndUpdateBankReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCloseBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, int bankReconciliationPeriodAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndCloseBankReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankReconciliationPeriods(AsyncCallback callback, Object state);
        List<BankReconciliationPeriodDTO> EndFindBankReconciliationPeriods(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankReconciliationPeriodsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankReconciliationPeriodDTO> EndFindBankReconciliationPeriodsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankReconciliationPeriod(Guid bankReconciliationPeriodId, AsyncCallback callback, Object state);
        BankReconciliationPeriodDTO EndFindBankReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBankReconciliationEntry(BankReconciliationEntryDTO bankReconciliationEntryDTO, AsyncCallback callback, Object state);
        BankReconciliationEntryDTO EndAddBankReconciliationEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveBankReconciliationEntries(List<BankReconciliationEntryDTO> bankReconciliationEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveBankReconciliationEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankReconciliationEntriesByBankReconciliationPeriodIdAndFilterInPage(Guid bankReconciliationPeriodId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankReconciliationEntryDTO> EndFindBankReconciliationEntriesByBankReconciliationPeriodIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankReconciliationPeriodsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankReconciliationPeriodDTO> EndFindBankReconciliationPeriodsByFilterInPage(IAsyncResult result);
    }
}
