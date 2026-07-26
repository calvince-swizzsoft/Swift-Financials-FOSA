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
    public class CostCenterService : ICostCenterService
    {
        private readonly ICostCenterAppService _costCenterAppService;

        public CostCenterService(
            ICostCenterAppService costCenterAppService)
        {
            Guard.ArgumentNotNull(costCenterAppService, nameof(costCenterAppService));

            _costCenterAppService = costCenterAppService;
        }

        #region Cost Center

        public CostCenterDTO AddCostCenter(CostCenterDTO costCenterDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _costCenterAppService.AddNewCostCenter(costCenterDTO, serviceHeader);
        }

        public bool UpdateCostCenter(CostCenterDTO costCenterDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _costCenterAppService.UpdateCostCenter(costCenterDTO, serviceHeader);
        }

        public List<CostCenterDTO> FindCostCenters()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _costCenterAppService.FindCostCenters(serviceHeader);
        }

        public CostCenterDTO FindCostCenter(Guid costCenterId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _costCenterAppService.FindCostCenter(costCenterId, serviceHeader);
        }

        public PageCollectionInfo<CostCenterDTO> FindCostCentersInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _costCenterAppService.FindCostCenters(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CostCenterDTO> FindCostCentersByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _costCenterAppService.FindCostCenters(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
