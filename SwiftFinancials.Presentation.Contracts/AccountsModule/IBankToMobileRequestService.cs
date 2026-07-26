using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IBankToMobileRequestService")]
    public interface IBankToMobileRequestService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBankToMobileRequest(BankToMobileRequestDTO bankToMobileRequestDTO, AsyncCallback callback, Object state);
        BankToMobileRequestDTO EndAddBankToMobileRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBankToMobileRequestResponse(Guid bankToMobileRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, AsyncCallback callback, Object state);
        bool EndUpdateBankToMobileRequestResponse(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBankToMobileRequestIPNStatus(Guid bankToMobileRequestId, int ipnStatus, string ipnResponse, AsyncCallback callback, Object state);
        bool EndUpdateBankToMobileRequestIPNStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginResetBankToMobileRequestsIPNStatus(Guid[] bankToMobileRequestIds, AsyncCallback callback, Object state);
        bool EndResetBankToMobileRequestsIPNStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankToMobileRequests(AsyncCallback callback, Object state);
        List<BankToMobileRequestDTO> EndFindBankToMobileRequests(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankToMobileRequest(Guid bankToMobileRequestId, AsyncCallback callback, Object state);
        BankToMobileRequestDTO EndFindBankToMobileRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankToMobileRequestsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankToMobileRequestDTO> EndFindBankToMobileRequestsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindThirdPartyNotifiableBankToMobileRequestsByFilterInPage(string text, int pageIndex, int pageSize, int daysCap, AsyncCallback callback, Object state);
        PageCollectionInfo<BankToMobileRequestDTO> EndFindThirdPartyNotifiableBankToMobileRequestsByFilterInPage(IAsyncResult result);
    }
}
