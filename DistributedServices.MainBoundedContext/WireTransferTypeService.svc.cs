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
    public class WireTransferTypeService : IWireTransferTypeService
    {
        private readonly IWireTransferTypeAppService _wireTransferTypeAppService;

        public WireTransferTypeService(
          IWireTransferTypeAppService wireTransferTypeAppService)
        {
            Guard.ArgumentNotNull(wireTransferTypeAppService, nameof(wireTransferTypeAppService));

            _wireTransferTypeAppService = wireTransferTypeAppService;
        }

        #region Wire Transfer Type

        public WireTransferTypeDTO AddWireTransferType(WireTransferTypeDTO wireTransferTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.AddNewWireTransferType(wireTransferTypeDTO, serviceHeader);
        }

        public bool UpdateWireTransferType(WireTransferTypeDTO wireTransferTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.UpdateWireTransferType(wireTransferTypeDTO, serviceHeader);
        }

        public List<WireTransferTypeDTO> FindWireTransferTypes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.FindWireTransferTypes(serviceHeader);
        }

        public PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.FindWireTransferTypes(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.FindWireTransferTypes(text, pageIndex, pageSize, serviceHeader);
        }

        public WireTransferTypeDTO FindWireTransferType(Guid wireTransferTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.FindWireTransferType(wireTransferTypeId, serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsByWireTransferTypeId(Guid wireTransferTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.FindCommissions(wireTransferTypeId, serviceHeader);
        }

        public bool UpdateCommissionsByWireTransferTypeId(Guid wireTransferTypeId, List<CommissionDTO> commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferTypeAppService.UpdateCommissions(wireTransferTypeId, commissions, serviceHeader);
        }
        
        #endregion
    }
}
