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
    public class SystemGeneralLedgerAccountMappingService : ISystemGeneralLedgerAccountMappingService
    {
        private readonly SystemGeneralLedgerAccountMappingAppService _systemGeneralLedgerAccountMappingAppService;

        public SystemGeneralLedgerAccountMappingService(
            SystemGeneralLedgerAccountMappingAppService systemGeneralLedgerAccountMappingAppService)
        {
            Guard.ArgumentNotNull(systemGeneralLedgerAccountMappingAppService, nameof(systemGeneralLedgerAccountMappingAppService));

            _systemGeneralLedgerAccountMappingAppService = systemGeneralLedgerAccountMappingAppService;
        }

        #region System General Ledger Account Mapping

        public SystemGeneralLedgerAccountMappingDTO AddSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _systemGeneralLedgerAccountMappingAppService.AddNewSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountMappingDTO, serviceHeader);
        }

        public bool UpdateSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _systemGeneralLedgerAccountMappingAppService.UpdateSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountMappingDTO, serviceHeader);
        }

        public List<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _systemGeneralLedgerAccountMappingAppService.FindSystemGeneralLedgerAccountMappings(serviceHeader);
        }

        public PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappingsInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _systemGeneralLedgerAccountMappingAppService.FindSystemGeneralLedgerAccountMappings(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappingsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _systemGeneralLedgerAccountMappingAppService.FindSystemGeneralLedgerAccountMappings(text, pageIndex, pageSize, serviceHeader);
        }

        public SystemGeneralLedgerAccountMappingDTO FindSystemGeneralLedgerAccountMapping(Guid systemGeneralLedgerAccountMappingId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _systemGeneralLedgerAccountMappingAppService.FindSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountMappingId, serviceHeader);
        }


        #endregion
    }
}
