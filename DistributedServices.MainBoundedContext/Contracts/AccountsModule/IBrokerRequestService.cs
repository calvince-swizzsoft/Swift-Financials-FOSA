using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IBrokerRequestService
    {
        #region Broker Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<BrokerRequestDTO> AddBrokerRequestAsync(BrokerRequestDTO brokerRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateBrokerRequestResponseAsync(Guid brokerRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateBrokerRequestIPNStatusAsync(Guid brokerRequestId, int ipnStatus, string ipnResponse);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> ResetBrokerRequestsIPNStatusAsync(Guid[] brokerRequestIds);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<BrokerRequestDTO>> FindBrokerRequestsAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<BrokerRequestDTO> FindBrokerRequestAsync(Guid brokerRequestId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<BrokerRequestDTO>> FindBrokerRequestsByDateRangeAndFilterInPageAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<BrokerRequestDTO>> FindThirdPartyNotifiableBrokerRequestsByFilterInPageAsync(string text, int pageIndex, int pageSize, int daysCap);

        #endregion
    }
}