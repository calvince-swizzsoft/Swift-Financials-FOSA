using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    public class WireTransferBatchService : IWireTransferBatchService
    {
        private readonly IWireTransferBatchAppService _wireTransferBatchAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public WireTransferBatchService(
            IWireTransferBatchAppService wireTransferBatchAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(wireTransferBatchAppService, nameof(wireTransferBatchAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _wireTransferBatchAppService = wireTransferBatchAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Wire Transfer Batch

        public WireTransferBatchDTO AddWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.AddNewWireTransferBatch(wireTransferBatchDTO, serviceHeader);
        }

        public bool UpdateWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.UpdateWireTransferBatch(wireTransferBatchDTO, serviceHeader);
        }

        public WireTransferBatchEntryDTO AddWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.AddNewWireTransferBatchEntry(wireTransferBatchEntryDTO, serviceHeader);
        }

        public bool RemoveWireTransferBatchEntries(List<WireTransferBatchEntryDTO> wireTransferBatchEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.RemoveWireTransferBatchEntries(wireTransferBatchEntryDTOs, serviceHeader);
        }

        public bool UpdateWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.UpdateWireTransferBatchEntry(wireTransferBatchEntryDTO, serviceHeader);
        }

        public bool AuditWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.AuditWireTransferBatch(wireTransferBatchDTO, batchAuthOption, serviceHeader);
        }

        public bool AuthorizeWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.AuthorizeWireTransferBatch(wireTransferBatchDTO, batchAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool PostWireTransferBatchEntry(Guid wireTransferBatchEntryId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.PostWireTransferBatchEntry(wireTransferBatchEntryId, moduleNavigationItemCode, serviceHeader);
        }

        public List<WireTransferBatchDTO> FindWireTransferBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.FindWireTransferBatches(serviceHeader);
        }

        public PageCollectionInfo<WireTransferBatchDTO> FindWireTransferBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.FindWireTransferBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public WireTransferBatchDTO FindWireTransferBatch(Guid wireTransferBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.FindWireTransferBatch(wireTransferBatchId, serviceHeader);
        }

        public List<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchId(Guid wireTransferBatchId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var wireTransferBatchEntries = _wireTransferBatchAppService.FindWireTransferBatchEntriesByWireTransferBatchId(wireTransferBatchId, serviceHeader);

            if (includeProductDescription && wireTransferBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(wireTransferBatchEntries, serviceHeader);

            return wireTransferBatchEntries;
        }

        public PageCollectionInfo<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchIdInPage(Guid wireTransferBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var wireTransferBatchEntries = _wireTransferBatchAppService.FindWireTransferBatchEntriesByWireTransferBatchId(wireTransferBatchId, text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && wireTransferBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(wireTransferBatchEntries.PageCollection, serviceHeader);

            return wireTransferBatchEntries;
        }

        public List<BatchImportEntryWrapper> ParseWireTransferBatchImport(Guid wireTransferBatchId, string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _wireTransferBatchAppService.ParseWireTransferBatchImport(wireTransferBatchId, serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);
        }

        public PageCollectionInfo<WireTransferBatchEntryDTO> FindQueableWireTransferBatchEntriesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _wireTransferBatchAppService.FindQueableWireTransferBatchEntries(pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
