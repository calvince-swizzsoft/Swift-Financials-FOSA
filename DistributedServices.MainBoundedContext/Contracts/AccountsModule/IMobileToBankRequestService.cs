using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IMobileToBankRequestService
    {
        #region Mobile To Bank Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MobileToBankRequestDTO AddMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ReconcileMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditMobileToBankRequestReconciliation(MobileToBankRequestDTO mobileToBankRequestDTO, int requestAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MobileToBankRequestDTO> FindMobileToBankRequests();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequestsByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequestsByStatusRecordStatusAndFilterInPage(int status, int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MobileToBankRequestDTO FindMobileToBankRequest(Guid mobileToBankRequestId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequestsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        #endregion
    }
}