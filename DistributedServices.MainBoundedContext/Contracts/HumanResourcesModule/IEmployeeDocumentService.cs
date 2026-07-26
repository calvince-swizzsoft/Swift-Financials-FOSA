using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmployeeDocumentService
    {
        #region  Employee Document

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDocumentDTO AddEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeDocumentDTO> FindEmployeeDocuments();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocumentsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocumentsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDocumentDTO FindEmployeeDocument(Guid employeeDocumentId);

        #endregion
    }
}
