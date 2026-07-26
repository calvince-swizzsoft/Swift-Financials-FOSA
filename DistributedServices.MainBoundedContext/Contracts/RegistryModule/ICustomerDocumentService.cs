using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ICustomerDocumentService
    {
        #region  Customer Document

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CustomerDocumentDTO> AddCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CustomerDocumentDTO>> FindCustomerDocumentsAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CustomerDocumentDTO>> FindCustomerDocumentsByCustomerIdAndTypeAsync(Guid customerId, int type);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsByFilterInPageAsync(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CustomerDocumentDTO> FindCustomerDocumentAsync(Guid customerDocumentId);

        #endregion
    }
}
