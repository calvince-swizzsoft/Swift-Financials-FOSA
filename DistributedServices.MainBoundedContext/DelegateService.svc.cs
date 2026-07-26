using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
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
    public class DelegateService : IDelegateService
    {
        private readonly IDelegateAppService _delegateAppService;

        public DelegateService(
            IDelegateAppService delegateAppService)
        {
            Guard.ArgumentNotNull(delegateAppService, nameof(delegateAppService));

            _delegateAppService = delegateAppService;
        }

        public DelegateDTO AddDelegate(DelegateDTO delegateDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _delegateAppService.AddNewDelegate(delegateDTO, serviceHeader);
        }

        public bool UpdateDelegate(DelegateDTO delegateDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _delegateAppService.UpdateDelegate(delegateDTO, serviceHeader);
        }

        public List<DelegateDTO> FindDelegates()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _delegateAppService.FindDelegates(serviceHeader);
        }

        public PageCollectionInfo<DelegateDTO> FindDelegatesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _delegateAppService.FindDelegates(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<DelegateDTO> FindDelegatesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _delegateAppService.FindDelegates(text, pageIndex, pageSize, serviceHeader);
        }

        public DelegateDTO FindDelegate(Guid delegateId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _delegateAppService.FindDelegate(delegateId, serviceHeader);
        }
    }
}
