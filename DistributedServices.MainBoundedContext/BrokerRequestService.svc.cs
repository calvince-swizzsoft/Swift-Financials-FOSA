using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.ErrorHandlers;
using System.ServiceModel;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using DistributedServices.Seedwork.EndpointBehaviors;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BrokerRequestService : IBrokerRequestService
    {
        private readonly IBrokerRequestAppService _brokerRequestAppService;

        public BrokerRequestService(
            IBrokerRequestAppService brokerRequestAppService)
        {
            Guard.ArgumentNotNull(brokerRequestAppService, nameof(brokerRequestAppService));

            _brokerRequestAppService = brokerRequestAppService;
        }

        #region Broker Request

        public async Task<BrokerRequestDTO> AddBrokerRequestAsync(BrokerRequestDTO brokerRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.AddNewBrokerRequestAsync(brokerRequestDTO, serviceHeader);
        }

        public async Task<BrokerRequestDTO> FindBrokerRequestAsync(Guid brokerRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.FindBrokerRequestAsync(brokerRequestId, serviceHeader);
        }

        public async Task<List<BrokerRequestDTO>> FindBrokerRequestsAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.FindBrokerRequestsAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<BrokerRequestDTO>> FindBrokerRequestsByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.FindBrokerRequestsAsync(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<BrokerRequestDTO>> FindThirdPartyNotifiableBrokerRequestsByFilterInPageAsync(string text, int pageIndex, int pageSize, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.FindThirdPartyNotifiableBrokerRequestsAsync(text, pageIndex, pageSize, daysCap, serviceHeader);
        }

        public async Task<bool> ResetBrokerRequestsIPNStatusAsync(Guid[] brokerRequestIds)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.ResetBrokerRequestsIPNStatusAsync(brokerRequestIds, serviceHeader);
        }

        public async Task<bool> UpdateBrokerRequestIPNStatusAsync(Guid brokerRequestId, int ipnStatus, string ipnResponse)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.UpdateBrokerRequestIPNStatusAsync(brokerRequestId, ipnStatus, ipnResponse, serviceHeader);
        }

        public async Task<bool> UpdateBrokerRequestResponseAsync(Guid brokerRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _brokerRequestAppService.UpdateBrokerRequestResponse(brokerRequestId, outgoingPlainTextPayload, outgoingCipherTextPayload, serviceHeader);
        }

        #endregion

    }
}