using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IFileRegisterService")]
    public interface IFileRegisterService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFileRegistersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FileRegisterDTO> EndFindFileRegistersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFileRegistersByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FileRegisterDTO> EndFindFileRegistersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFileRegistersByFilterStatusAndLastFileMovementDestinationDepartmentIdInPage(string text, int customerFilter, int fileMovementStatus, Guid lastDestinationDepartmentId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FileRegisterDTO> EndFindFileRegistersByFilterStatusAndLastFileMovementDestinationDepartmentIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFileRegistersByFilterExcludingLastDestinationDepartmentIdInPage(string text, int customerFilter, Guid lastDestinationDepartmentId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FileRegisterDTO> EndFindFileRegistersByFilterExcludingLastDestinationDepartmentIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFileMovementHistoryByFileRegisterIdInPage(Guid fileRegisterId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FileMovementHistoryDTO> EndFindFileMovementHistoryByFileRegisterIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFileMovementHistoryByFileRegisterId(Guid fileRegisterId, AsyncCallback callback, Object state);
        List<FileMovementHistoryDTO> EndFindFileMovementHistoryByFileRegisterId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFileRegisterAndLastDepartmentByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        CustomerFileRegisterLastDepartmentInfo EndFindFileRegisterAndLastDepartmentByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMultiDestinationDispatch(List<FileMovementHistoryDTO> fileMovementHistoryDTOs, AsyncCallback callback, Object state);
        bool EndMultiDestinationDispatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginSingleDestinationDispatch(Guid sourceDepartmentId, Guid destinationDepartmentId, string remarks, string carrier, List<FileRegisterDTO> fileRegisterDTOs, AsyncCallback callback, Object state);
        bool EndSingleDestinationDispatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginReceiveFiles(List<FileRegisterDTO> fileRegisterDTOs, AsyncCallback callback, Object state);
        bool EndReceiveFiles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRecallFiles(List<FileRegisterDTO> fileRegisterDTOs, AsyncCallback callback, Object state);
        bool EndRecallFiles(IAsyncResult result);
    }
}
