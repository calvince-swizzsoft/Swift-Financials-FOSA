using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CustomerDocumentService : ICustomerDocumentService
    {
        private readonly ICustomerDocumentAppService _customerDocumentAppService;

        public CustomerDocumentService(
            ICustomerDocumentAppService customerDocumentAppService)
        {
            Guard.ArgumentNotNull(customerDocumentAppService, nameof(customerDocumentAppService));

            _customerDocumentAppService = customerDocumentAppService;
        }

        #region Customer Document

        public async Task<CustomerDocumentDTO> AddCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return await _customerDocumentAppService.AddNewCustomerDocumentAsync(customerDocumentDTO, serviceBrokerSettingsElement.FileUploadDirectory, serviceHeader);
        }

        public async Task<bool> UpdateCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return await _customerDocumentAppService.UpdateCustomerDocumentAsync(customerDocumentDTO, serviceBrokerSettingsElement.FileUploadDirectory, serviceHeader);
        }

        public async Task<List<CustomerDocumentDTO>> FindCustomerDocumentsAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerDocumentAppService.FindCustomerDocumentsAsync(serviceHeader);
        }

        public async Task<List<CustomerDocumentDTO>> FindCustomerDocumentsByCustomerIdAndTypeAsync(Guid customerId, int type)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerDocumentAppService.FindCustomerDocumentsAsync(customerId, type, serviceHeader);
        }

        public async Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerDocumentAppService.FindCustomerDocumentsAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsByFilterInPageAsync(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerDocumentAppService.FindCustomerDocumentsAsync(text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<CustomerDocumentDTO> FindCustomerDocumentAsync(Guid customerDocumentId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerDocumentAppService.FindCustomerDocumentAsync(customerDocumentId, serviceHeader);
        }

        #endregion
    }
}
