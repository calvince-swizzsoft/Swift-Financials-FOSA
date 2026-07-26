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
    public class TreasuryService : ITreasuryService
    {
        private readonly ITreasuryAppService _treasuryAppService;

        public TreasuryService(
           ITreasuryAppService treasuryAppService)
        {
            Guard.ArgumentNotNull(treasuryAppService, nameof(treasuryAppService));

            _treasuryAppService = treasuryAppService;
        }

        #region Treasury

        public TreasuryDTO AddTreasury(TreasuryDTO treasuryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _treasuryAppService.AddNewTreasury(treasuryDTO, serviceHeader);
        }

        public bool UpdateTreasury(TreasuryDTO treasuryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _treasuryAppService.UpdateTreasury(treasuryDTO, serviceHeader);
        }

        public List<TreasuryDTO> FindTreasuries(bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var treasuries = _treasuryAppService.FindTreasuries(serviceHeader);

            if (treasuries != null)
            {
                if (includeBalances)
                    _treasuryAppService.FetchTreasuryBalances(treasuries, serviceHeader);
            }

            return treasuries;
        }

        public PageCollectionInfo<TreasuryDTO> FindTreasuriesInPage(int pageIndex, int pageSize, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var treasuries = _treasuryAppService.FindTreasuries(pageIndex, pageSize, serviceHeader);

            if (treasuries != null)
            {
                if (includeBalances)
                    _treasuryAppService.FetchTreasuryBalances(treasuries.PageCollection, serviceHeader);
            }

            return treasuries;
        }

        public PageCollectionInfo<TreasuryDTO> FindTreasuriesByFilterInPage(string text, int pageIndex, int pageSize, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var treasuries = _treasuryAppService.FindTreasuries(text, pageIndex, pageSize, serviceHeader);

            if (treasuries != null)
            {
                if (includeBalances)
                    _treasuryAppService.FetchTreasuryBalances(treasuries.PageCollection, serviceHeader);
            }

            return treasuries;
        }

        public TreasuryDTO FindTreasury(Guid treasuryId, bool includeBalance)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var treasury = _treasuryAppService.FindTreasury(treasuryId, serviceHeader);

            if (treasury != null)
            {
                if (includeBalance)
                    _treasuryAppService.FetchTreasuryBalances(new List<TreasuryDTO> { treasury }, serviceHeader);
            }

            return treasury;
        }

        public TreasuryDTO FindTreasuryByBranchId(Guid branchId, bool includeBalance)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var treasury = _treasuryAppService.FindTreasuryByBranchId(branchId, serviceHeader);

            if (treasury != null)
            {
                if (includeBalance)
                    _treasuryAppService.FetchTreasuryBalances(new List<TreasuryDTO> { treasury }, serviceHeader);
            }

            return treasury;
        }

        #endregion
    }
}
