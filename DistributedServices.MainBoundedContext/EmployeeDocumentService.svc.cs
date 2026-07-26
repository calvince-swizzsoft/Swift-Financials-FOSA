using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class EmployeeDocumentService : IEmployeeDocumentService
    {
        private readonly IEmployeeDocumentAppService _employeeDocumentAppService;

        public EmployeeDocumentService(
            IEmployeeDocumentAppService employeeDocumentAppService)
        {
            Guard.ArgumentNotNull(employeeDocumentAppService, nameof(employeeDocumentAppService));

            _employeeDocumentAppService = employeeDocumentAppService;
        }

        #region Employee Document

        public EmployeeDocumentDTO AddEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _employeeDocumentAppService.AddNewEmployeeDocument(employeeDocumentDTO, serviceBrokerSettingsElement.FileUploadDirectory, serviceHeader);
        }

        public bool UpdateEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader); 

            return _employeeDocumentAppService.UpdateEmployeeDocument(employeeDocumentDTO, serviceBrokerSettingsElement.FileUploadDirectory, serviceHeader);
        }

        public List<EmployeeDocumentDTO> FindEmployeeDocuments()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDocumentAppService.FindEmployeeDocuments(serviceHeader);
        }

        public PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocumentsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDocumentAppService.FindEmployeeDocuments(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocumentsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDocumentAppService.FindEmployeeDocuments(text, pageIndex, pageSize, serviceHeader);
        }

        public EmployeeDocumentDTO FindEmployeeDocument(Guid employeeDocumentId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDocumentAppService.FindEmployeeDocument(employeeDocumentId, serviceHeader);
        }

        #endregion
    }
}
