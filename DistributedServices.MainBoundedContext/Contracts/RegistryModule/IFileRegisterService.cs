using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IFileRegisterService
    {
        #region File Register

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FileRegisterDTO> FindFileRegistersInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FileRegisterDTO> FindFileRegistersByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FileRegisterDTO> FindFileRegistersByFilterStatusAndLastFileMovementDestinationDepartmentIdInPage(string text, int customerFilter, int fileMovementStatus, Guid lastDestinationDepartmentId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FileRegisterDTO> FindFileRegistersByFilterExcludingLastDestinationDepartmentIdInPage(string text, int customerFilter, Guid lastDestinationDepartmentId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterIdInPage(Guid fileRegisterId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterId(Guid fileRegisterId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CustomerFileRegisterLastDepartmentInfo FindFileRegisterAndLastDepartmentByCustomerId(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MultiDestinationDispatch(List<FileMovementHistoryDTO> fileMovementHistoryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool SingleDestinationDispatch(Guid sourceDepartmentId, Guid destinationDepartmentId, string remarks, string carrier, List<FileRegisterDTO> fileRegisterDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ReceiveFiles(List<FileRegisterDTO> fileRegisterDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RecallFiles(List<FileRegisterDTO> fileRegisterDTOs);

        #endregion
    }
}
