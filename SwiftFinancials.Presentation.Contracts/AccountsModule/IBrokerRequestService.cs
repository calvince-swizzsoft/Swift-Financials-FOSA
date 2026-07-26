using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IBrokerRequestService")]
    public interface IBrokerRequestService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBrokerRequest(BrokerRequestDTO brokerRequestDTO, AsyncCallback callback, Object state);
        BrokerRequestDTO EndAddBrokerRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBrokerRequestResponse(Guid brokerRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, AsyncCallback callback, Object state);
        bool EndUpdateBrokerRequestResponse(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBrokerRequestIPNStatus(Guid brokerRequestId, int ipnStatus, string ipnResponse, AsyncCallback callback, Object state);
        bool EndUpdateBrokerRequestIPNStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginResetBrokerRequestsIPNStatus(Guid[] brokerRequestIds, AsyncCallback callback, Object state);
        bool EndResetBrokerRequestsIPNStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBrokerRequests(AsyncCallback callback, Object state);
        List<BrokerRequestDTO> EndFindBrokerRequests(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBrokerRequest(Guid brokerRequestId, AsyncCallback callback, Object state);
        BrokerRequestDTO EndFindBrokerRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBrokerRequestsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BrokerRequestDTO> EndFindBrokerRequestsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindThirdPartyNotifiableBrokerRequestsByFilterInPage(string text, int pageIndex, int pageSize, int daysCap, AsyncCallback callback, Object state);
        PageCollectionInfo<BrokerRequestDTO> EndFindThirdPartyNotifiableBrokerRequestsByFilterInPage(IAsyncResult result);
    }
}