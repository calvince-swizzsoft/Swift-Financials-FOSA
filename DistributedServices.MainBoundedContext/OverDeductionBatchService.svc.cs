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
    public class OverDeductionBatchService : IOverDeductionBatchService
    {
        private readonly IOverDeductionBatchAppService _overDeductionBatchAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public OverDeductionBatchService(
            IOverDeductionBatchAppService overDeductionBatchAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(overDeductionBatchAppService, nameof(overDeductionBatchAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _overDeductionBatchAppService = overDeductionBatchAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Over Deduction Batch

        public OverDeductionBatchDTO AddOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.AddNewOverDeductionBatch(overDeductionBatchDTO, serviceHeader);
        }

        public bool UpdateOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.UpdateOverDeductionBatch(overDeductionBatchDTO, serviceHeader);
        }

        public OverDeductionBatchEntryDTO AddOverDeductionBatchEntry(OverDeductionBatchEntryDTO overDeductionBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.AddNewOverDeductionBatchEntry(overDeductionBatchEntryDTO, serviceHeader);
        }

        public bool RemoveOverDeductionBatchEntries(List<OverDeductionBatchEntryDTO> overDeductionBatchEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.RemoveOverDeductionBatchEntries(overDeductionBatchEntryDTOs, serviceHeader);
        }

        public bool AuditOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.AuditOverDeductionBatch(overDeductionBatchDTO, batchAuthOption, serviceHeader);
        }

        public bool AuthorizeOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.AuthorizeOverDeductionBatch(overDeductionBatchDTO, batchAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public List<BatchImportEntryWrapper> ParseOverDeductionBatchImport(Guid overDeductionBatchId, string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _overDeductionBatchAppService.ParseOverDeductionBatchImport(overDeductionBatchId, serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);
        }

        public List<OverDeductionBatchDTO> FindOverDeductionBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.FindOverDeductionBatches(serviceHeader);
        }

        public PageCollectionInfo<OverDeductionBatchDTO> FindOverDeductionBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.FindOverDeductionBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public OverDeductionBatchDTO FindOverDeductionBatch(Guid overDeductionBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _overDeductionBatchAppService.FindOverDeductionBatch(overDeductionBatchId, serviceHeader);
        }

        public List<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchId(Guid overDeductionBatchId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var overDeductionBatchEntries = _overDeductionBatchAppService.FindOverDeductionBatchEntriesByOverDeductionBatchId(overDeductionBatchId, serviceHeader);

            if (includeProductDescription && overDeductionBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(overDeductionBatchEntries, serviceHeader);

            return overDeductionBatchEntries;
        }

        public PageCollectionInfo<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchIdInPage(Guid overDeductionBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var overDeductionBatchEntries = _overDeductionBatchAppService.FindOverDeductionBatchEntriesByOverDeductionBatchId(overDeductionBatchId, text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && overDeductionBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(overDeductionBatchEntries.PageCollection, serviceHeader);

            return overDeductionBatchEntries;
        }

        #endregion
    }
}
