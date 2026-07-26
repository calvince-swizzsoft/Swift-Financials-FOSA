using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IBrokerRequestAppService
    {
        Task<BrokerRequestDTO> AddNewBrokerRequestAsync(BrokerRequestDTO brokerRequestDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateBrokerRequestResponse(Guid brokerRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, ServiceHeader serviceHeader);

        Task<bool> UpdateBrokerRequestIPNStatusAsync(Guid brokerRequestId, int ipnStatus, string ipnResponse, ServiceHeader serviceHeader);

        Task<bool> ResetBrokerRequestsIPNStatusAsync(Guid[] brokerRequestIds, ServiceHeader serviceHeader);

        Task<List<BrokerRequestDTO>> FindBrokerRequestsAsync(ServiceHeader serviceHeader);

        Task<List<BrokerRequestDTO>> FindBrokerRequestsAsync(Guid[] brokerRequestIds, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<BrokerRequestDTO>> FindBrokerRequestsAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<BrokerRequestDTO>> FindThirdPartyNotifiableBrokerRequestsAsync(string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader);

        Task<BrokerRequestDTO> FindBrokerRequestAsync(Guid brokerRequestId, ServiceHeader serviceHeader);
    }
}