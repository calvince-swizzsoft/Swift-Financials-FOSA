using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "ICashTransferRequestService")]
    public interface ICashTransferRequestService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCashTransferRequest(CashTransferRequestDTO cashTransferRequestDTO, AsyncCallback callback, Object state);
        CashTransferRequestDTO EndAddCashTransferRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAcknowledgeCashTransferRequest(CashTransferRequestDTO cashTransferRequestDTO, int cashTransferRequestAcknowledgeOption, AsyncCallback callback, Object state);
        bool EndAcknowledgeCashTransferRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashTransferRequests(AsyncCallback callback, Object state);
        List<CashTransferRequestDTO> EndFindCashTransferRequests(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMatureCashTransferRequestsByEmployeeId(Guid employeeId, AsyncCallback callback, Object state);
        List<CashTransferRequestDTO> EndFindMatureCashTransferRequestsByEmployeeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashTransferRequestsByFilterInPage(Guid employeeId, DateTime startDate, DateTime endDate, int status, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CashTransferRequestDTO> EndFindCashTransferRequestsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashTransferRequestsByStatusAndFilterInPage(string text, DateTime startDate, DateTime endDate, int status, int customerFlter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CashTransferRequestDTO> EndFindCashTransferRequestsByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashTransferRequest(Guid cashTransferRequestId, AsyncCallback callback, Object state);
        CashTransferRequestDTO EndFindCashTransferRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUtilizeCashTransferRequest(Guid cashTransferRequestId, AsyncCallback callback, Object state);
        bool EndUtilizeCashTransferRequest(IAsyncResult result);
    }
}
