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
    public class DynamicChargeService : IDynamicChargeService
    {
        private readonly IDynamicChargeAppService _dynamicChargeAppService;

        public DynamicChargeService(
           IDynamicChargeAppService dynamicChargeAppService)
        {
            Guard.ArgumentNotNull(dynamicChargeAppService, nameof(dynamicChargeAppService));

            _dynamicChargeAppService = dynamicChargeAppService;
        }

        #region Dynamic Charge

        public DynamicChargeDTO AddDynamicCharge(DynamicChargeDTO dynamicChargeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.AddNewDynamicCharge(dynamicChargeDTO, serviceHeader);
        }

        public bool UpdateDynamicCharge(DynamicChargeDTO dynamicChargeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.UpdateDynamicCharge(dynamicChargeDTO, serviceHeader);
        }

        public List<DynamicChargeDTO> FindDynamicCharges()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.FindDynamicCharges(serviceHeader);
        }

        public DynamicChargeDTO FindDynamicCharge(Guid dynamicChargeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.FindDynamicCharge(dynamicChargeId, serviceHeader);
        }

        public PageCollectionInfo<DynamicChargeDTO> FindDynamicChargesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.FindDynamicCharges(pageIndex, pageSize, serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsByDynamicChargeId(Guid dynamicChargeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.FindCommissions(dynamicChargeId, serviceHeader);
        }

        public bool UpdateCommissionsByDynamicChargeId(Guid dynamicChargeId, List<CommissionDTO> commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.UpdateCommissions(dynamicChargeId, commissions, serviceHeader);
        }

        public PageCollectionInfo<DynamicChargeDTO> FindDynamicChargesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dynamicChargeAppService.FindDynamicCharges(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
