using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IMobileToBankRequestService")]
    public interface IMobileToBankRequestService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO, AsyncCallback callback, Object state);
        MobileToBankRequestDTO EndAddMobileToBankRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginReconcileMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO, AsyncCallback callback, Object state);
        bool EndReconcileMobileToBankRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditMobileToBankRequestReconciliation(MobileToBankRequestDTO mobileToBankRequestDTO, int requestAuthOption, AsyncCallback callback, Object state);
        bool EndAuditMobileToBankRequestReconciliation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMobileToBankRequests(AsyncCallback callback, Object state);
        List<MobileToBankRequestDTO> EndFindMobileToBankRequests(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMobileToBankRequestsByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MobileToBankRequestDTO> EndFindMobileToBankRequestsByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMobileToBankRequestsByStatusRecordStatusAndFilterInPage(int status, int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MobileToBankRequestDTO> EndFindMobileToBankRequestsByStatusRecordStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMobileToBankRequest(Guid mobileToBankRequestId, AsyncCallback callback, Object state);
        MobileToBankRequestDTO EndFindMobileToBankRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMobileToBankRequestsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MobileToBankRequestDTO> EndFindMobileToBankRequestsByDateRangeAndFilterInPage(IAsyncResult result);
    }
}
