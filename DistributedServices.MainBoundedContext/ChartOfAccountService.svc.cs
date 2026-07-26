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
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ChartOfAccountService : IChartOfAccountService
    {
        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        public ChartOfAccountService(
            IChartOfAccountAppService chartOfAccountAppService)
        {
            Guard.ArgumentNotNull(chartOfAccountAppService, nameof(chartOfAccountAppService));

            _chartOfAccountAppService = chartOfAccountAppService;
        }

        //#region Chart Of Account

        public Task<PageCollectionInfo<ChartOfAccountDTO>> FindChartOfAccountsByFilterInPageAsync(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return Task.FromResult(_chartOfAccountAppService.FindChartOfAccounts(text, pageIndex, pageSize, serviceHeader));
        }

        public List<ChartOfAccountDTO> FindChartOfAccounts()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chartOfAccountAppService.FindChartOfAccounts(serviceHeader);
        }

        public ChartOfAccountDTO AddChartOfAccount(ChartOfAccountDTO chartOfAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chartOfAccountAppService.AddNewChartOfAccount(chartOfAccountDTO, serviceHeader);
        }

        public bool UpdateChartOfAccount(ChartOfAccountDTO chartOfAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chartOfAccountAppService.UpdateChartOfAccount(chartOfAccountDTO, serviceHeader);
        }

        public ChartOfAccountDTO FindChartOfAccount(Guid chartOfAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chartOfAccountAppService.FindChartOfAccount(chartOfAccountId, serviceHeader);
        }

        public List<GeneralLedgerAccount> FindGeneralLedgerAccounts(bool includeBalances, bool updateDepth)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var glAccounts = _chartOfAccountAppService.FindGeneralLedgerAccounts(serviceHeader, updateDepth);

            if (includeBalances)
                _chartOfAccountAppService.FetchGeneralLedgerAccountBalances(glAccounts, DateTime.Now, serviceHeader, TransactionDateFilter.CreatedDate, true);

            return glAccounts;
        }

        public List<GeneralLedgerAccount> FindGeneralLedgerAccountsWithCategoryAndText(string text, int? accountCategory, bool updateDepth)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var glAccounts = _chartOfAccountAppService.FindGeneralLedgerAccountsWithCategoryAndText(text, accountCategory, serviceHeader, updateDepth);

           
             _chartOfAccountAppService.FetchGeneralLedgerAccountBalances(glAccounts, DateTime.Now, serviceHeader, TransactionDateFilter.CreatedDate, true);

            return glAccounts;
        }

        public GeneralLedgerAccount FindGeneralLedgerAccount(Guid chartOfAccountId, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var glAccount = _chartOfAccountAppService.FindGeneralLedgerAccount(chartOfAccountId, serviceHeader);

            if (includeBalances)
                _chartOfAccountAppService.FetchGeneralLedgerAccountBalances(glAccount, DateTime.Now, serviceHeader, TransactionDateFilter.CreatedDate);

            return glAccount;
        }

        public Guid GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(systemGeneralLedgerAccountCode, serviceHeader);
        }

        public bool MapSystemGeneralLedgerAccountCodeToChartOfAccount(int systemGeneralLedgerAccountCode, Guid chartOfAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chartOfAccountAppService.MapSystemGeneralLedgerAccountCodeToChartOfAccount(systemGeneralLedgerAccountCode, chartOfAccountId, serviceHeader);
        }

        public PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappingsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chartOfAccountAppService.FindSystemGeneralLedgerAccountMappings(pageIndex, pageSize, serviceHeader);
        }



    }

}
