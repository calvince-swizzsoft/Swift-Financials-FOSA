using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IChartOfAccountService")]
    public interface IChartOfAccountService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChartOfAccountsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ChartOfAccountDTO> EndFindChartOfAccountsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChartOfAccounts(AsyncCallback callback, Object state);
        List<ChartOfAccountDTO> EndFindChartOfAccounts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddChartOfAccount(ChartOfAccountDTO chartOfAccountDTO, AsyncCallback callback, Object state);
        ChartOfAccountDTO EndAddChartOfAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateChartOfAccount(ChartOfAccountDTO chartOfAccountDTO, AsyncCallback callback, Object state);
        bool EndUpdateChartOfAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChartOfAccount(Guid chartOfAccountId, AsyncCallback callback, Object state);
        ChartOfAccountDTO EndFindChartOfAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerAccounts(bool includeBalances, bool updateDepth, AsyncCallback callback, Object state);
        List<GeneralLedgerAccount> EndFindGeneralLedgerAccounts(IAsyncResult result);



        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindGeneralLedgerAccountsWithCategoryAndText(string text, int? accountCategory, bool updateDepth, AsyncCallback callback, Object state);
        List<GeneralLedgerAccount> EndFindGeneralLedgerAccountsWithCategoryAndText(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerAccount(Guid chartOfAccountId, bool includeBalances, AsyncCallback callback, Object state);
        GeneralLedgerAccount EndFindGeneralLedgerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetChartOfAccountMappingForSystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode, AsyncCallback callback, Object state);
        Guid EndGetChartOfAccountMappingForSystemGeneralLedgerAccountCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMapSystemGeneralLedgerAccountCodeToChartOfAccount(int systemGeneralLedgerAccountCode, Guid chartOfAccountId, AsyncCallback callback, Object state);
        bool EndMapSystemGeneralLedgerAccountCodeToChartOfAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSystemGeneralLedgerAccountMappingsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> EndFindSystemGeneralLedgerAccountMappingsInPage(IAsyncResult result);
    }
}
