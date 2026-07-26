using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IEmployeeDocumentService")]
    public interface IEmployeeDocumentService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO, AsyncCallback callback, Object state);
        EmployeeDocumentDTO EndAddEmployeeDocument(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployeeDocument(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDocuments(AsyncCallback callback, Object state);
        List<EmployeeDocumentDTO> EndFindEmployeeDocuments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDocumentsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDocumentDTO> EndFindEmployeeDocumentsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDocumentsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDocumentDTO> EndFindEmployeeDocumentsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDocument(Guid employeeDocumentId, AsyncCallback callback, Object state);
        EmployeeDocumentDTO EndFindEmployeeDocument(IAsyncResult result);
    }
}
