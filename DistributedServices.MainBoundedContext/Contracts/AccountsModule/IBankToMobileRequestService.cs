using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IBankToMobileRequestService
    {
        #region Bank To Mobile Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankToMobileRequestDTO AddBankToMobileRequest(BankToMobileRequestDTO bankToMobileRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateBankToMobileRequestResponse(Guid bankToMobileRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateBankToMobileRequestIPNStatus(Guid bankToMobileRequestId, int ipnStatus, string ipnResponse);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ResetBankToMobileRequestsIPNStatus(Guid[] bankToMobileRequestIds);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BankToMobileRequestDTO> FindBankToMobileRequests();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankToMobileRequestDTO FindBankToMobileRequest(Guid bankToMobileRequestId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankToMobileRequestDTO> FindBankToMobileRequestsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankToMobileRequestDTO> FindThirdPartyNotifiableBankToMobileRequestsByFilterInPage(string text, int pageIndex, int pageSize, int daysCap);

        #endregion
    }
}
