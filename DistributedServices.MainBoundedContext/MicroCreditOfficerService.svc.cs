using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.MainBoundedContext.MicroCreditModule.Services;
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
    public class MicroCreditOfficerService : IMicroCreditOfficerService
    {
        private readonly IMicroCreditOfficerAppService _microCreditOfficerAppService;

        public MicroCreditOfficerService(
            IMicroCreditOfficerAppService microCreditOfficerAppService)
        {
            Guard.ArgumentNotNull(microCreditOfficerAppService, nameof(microCreditOfficerAppService));

            _microCreditOfficerAppService = microCreditOfficerAppService;
        }

        #region MicroCreditOfficer

        public MicroCreditOfficerDTO AddMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditOfficerAppService.AddNewMicroCreditOfficer(microCreditOfficerDTO, serviceHeader);
        }

        public bool UpdateMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditOfficerAppService.UpdateMicroCreditOfficer(microCreditOfficerDTO, serviceHeader);
        }

        public List<MicroCreditOfficerDTO> FindMicroCreditOfficers()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditOfficerAppService.FindMicroCreditOfficers(serviceHeader);
        }

        public PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficersInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditOfficerAppService.FindMicroCreditOfficers(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficersByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditOfficerAppService.FindMicroCreditOfficers(text, pageIndex, pageSize, serviceHeader);
        }

        public MicroCreditOfficerDTO FindMicroCreditOfficer(Guid microCreditOfficerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditOfficerAppService.FindMicroCreditOfficer(microCreditOfficerId, serviceHeader);
        }

        #endregion
    }
}
