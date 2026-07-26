using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using System;
using System.Collections.Generic;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface ICustomerDocumentAppService
    {
        CustomerDocumentDTO AddNewCustomerDocument(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader);

        Task<CustomerDocumentDTO> AddNewCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader);

        bool UpdateCustomerDocument(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader);

        Task<bool> UpdateCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader);

        bool UpdateCollateralStatus(Guid customerDocumentId, int collateralStatus, ServiceHeader serviceHeader);

        Task<bool> UpdateCollateralStatusAsync(Guid customerDocumentId, int collateralStatus, ServiceHeader serviceHeader);

        List<CustomerDocumentDTO> FindCustomerDocuments(ServiceHeader serviceHeader);

        Task<List<CustomerDocumentDTO>> FindCustomerDocumentsAsync(ServiceHeader serviceHeader);

        List<CustomerDocumentDTO> FindCustomerDocuments(Guid customerId, int type, ServiceHeader serviceHeader);

        Task<List<CustomerDocumentDTO>> FindCustomerDocumentsAsync(Guid customerId, int type, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerDocumentDTO> FindCustomerDocuments(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerDocumentDTO> FindCustomerDocuments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CustomerDocumentDTO FindCustomerDocument(Guid customerDocumentId, ServiceHeader serviceHeader);

        Task<CustomerDocumentDTO> FindCustomerDocumentAsync(Guid customerDocumentId, ServiceHeader serviceHeader);
    }
}
