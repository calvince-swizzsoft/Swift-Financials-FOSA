using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IChartOfAccountAppService
    {
        Task<PageCollectionInfo<ChartOfAccountDTO>> FindChartOfAccountsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);
       
        PageCollectionInfo<ChartOfAccountDTO> FindChartOfAccounts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ChartOfAccountDTO AddNewChartOfAccount(ChartOfAccountDTO chartOfAccountDTO, ServiceHeader serviceHeader);

        bool UpdateChartOfAccount(ChartOfAccountDTO chartOfAccountDTO, ServiceHeader serviceHeader);

        ChartOfAccountDTO FindChartOfAccount(Guid chartOfAccountId, ServiceHeader serviceHeader);

        List<ChartOfAccountSummaryDTO> FindChartOfAccounts(Guid[] chartOfAccountIds, ServiceHeader serviceHeader);

        Task<List<ChartOfAccountSummaryDTO>> FindChartOfAccountsAsync(Guid[] chartOfAccountIds, ServiceHeader serviceHeader);

        ChartOfAccountSummaryDTO FindCachedChartOfAccountSummary(Guid chartOfAccountId, ServiceHeader serviceHeader);

        ChartOfAccountSummaryDTO FindChartOfAccountSummary(Guid chartOfAccountId, ServiceHeader serviceHeader);

        List<ChartOfAccountDTO> FindChartOfAccounts(ServiceHeader serviceHeader);

        List<ChartOfAccountDTO> FindChartOfAccounts(int chartOfAccountCode, ServiceHeader serviceHeader);

        List<GeneralLedgerAccount> FindGeneralLedgerAccounts(ServiceHeader serviceHeader, bool updateDepth = false);

        GeneralLedgerAccount FindGeneralLedgerAccount(Guid chartOfAccountId, ServiceHeader serviceHeader);

        Guid GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode, ServiceHeader serviceHeader);

        Guid GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode, ServiceHeader serviceHeader);

        Guid GetCostCenterMappingForChartOfAccount(Guid chartOfAccountId, ServiceHeader serviceHeader);

        Guid GetCachedCostCenterMappingForChartOfAccount(Guid chartOfAccountId, ServiceHeader serviceHeader);

        bool MapSystemGeneralLedgerAccountCodeToChartOfAccount(int systemGeneralLedgerAccountCode, Guid chartOfAccountId, ServiceHeader serviceHeader);

        List<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(ServiceHeader serviceHeader);

        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        void FetchGeneralLedgerAccountBalances(List<GeneralLedgerAccount> glAccounts, DateTime cutOffDate, ServiceHeader serviceHeader, TransactionDateFilter transactionDateFilter = TransactionDateFilter.CreatedDate, bool includeSubTotals = false);

        void FetchGeneralLedgerAccountBalances(Guid branchId, List<GeneralLedgerAccount> glAccounts, DateTime cutOffDate, ServiceHeader serviceHeader, TransactionDateFilter transactionDateFilter = TransactionDateFilter.CreatedDate, bool includeSubTotals = false);

        void FetchGeneralLedgerAccountBalances(GeneralLedgerAccount glAccount, DateTime cutOffDate, ServiceHeader serviceHeader, TransactionDateFilter transactionDateFilter = TransactionDateFilter.CreatedDate);

        List<GeneralLedgerAccount> FindGeneralLedgerAccountsWithCategoryAndText(string text, int? accountCategory, ServiceHeader serviceHeader, bool updateDepth);
  

    }
}
