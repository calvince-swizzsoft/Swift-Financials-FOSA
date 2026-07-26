using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "ICustomerDocumentService")]
    public interface ICustomerDocumentService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCustomerDocument(CustomerDocumentDTO customerDocumentDTO, AsyncCallback callback, Object state);
        CustomerDocumentDTO EndAddCustomerDocument(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCustomerDocument(CustomerDocumentDTO customerDocumentDTO, AsyncCallback callback, Object state);
        bool EndUpdateCustomerDocument(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerDocuments(AsyncCallback callback, Object state);
        List<CustomerDocumentDTO> EndFindCustomerDocuments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerDocumentsByCustomerIdAndType(Guid customerId, int type, AsyncCallback callback, Object state);
        List<CustomerDocumentDTO> EndFindCustomerDocumentsByCustomerIdAndType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerDocumentsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerDocumentDTO> EndFindCustomerDocumentsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerDocumentsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerDocumentDTO> EndFindCustomerDocumentsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerDocument(Guid customerDocumentId, AsyncCallback callback, Object state);
        CustomerDocumentDTO EndFindCustomerDocument(IAsyncResult result);
    }
}
