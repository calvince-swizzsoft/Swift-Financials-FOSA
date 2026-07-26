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
    public class DebitBatchService : IDebitBatchService
    {
        private readonly IDebitBatchAppService _debitBatchAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public DebitBatchService(
            IDebitBatchAppService debitBatchAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(debitBatchAppService, nameof(debitBatchAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _debitBatchAppService = debitBatchAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Debit Batch

        public DebitBatchDTO AddDebitBatch(DebitBatchDTO debitBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.AddNewDebitBatch(debitBatchDTO, serviceHeader);
        }

        public bool UpdateDebitBatch(DebitBatchDTO debitBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.UpdateDebitBatch(debitBatchDTO, serviceHeader);
        }

        public DebitBatchEntryDTO AddDebitBatchEntry(DebitBatchEntryDTO debitBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.AddNewDebitBatchEntry(debitBatchEntryDTO, serviceHeader);
        }

        public bool RemoveDebitBatchEntries(List<DebitBatchEntryDTO> debitBatchEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.RemoveDebitBatchEntries(debitBatchEntryDTOs, serviceHeader);
        }

        public bool AuditDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.AuditDebitBatch(debitBatchDTO, batchAuthOption, serviceHeader);
        }

        public bool AuthorizeDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.AuthorizeDebitBatch(debitBatchDTO, batchAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool PostDebitBatchEntry(Guid debitBatchEntryId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.PostDebitBatchEntry(debitBatchEntryId, moduleNavigationItemCode, serviceHeader);
        }

        public List<DebitBatchDTO> FindDebitBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.FindDebitBatches(serviceHeader);
        }

        public PageCollectionInfo<DebitBatchDTO> FindDebitBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.FindDebitBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<DebitBatchDTO> FindDebitBatchesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.FindDebitBatches(text, pageIndex, pageSize, serviceHeader);
        }


        public DebitBatchDTO FindDebitBatch(Guid debitBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.FindDebitBatch(debitBatchId, serviceHeader);
        }

        public List<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchId(Guid debitBatchId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var debitBatchEntries = _debitBatchAppService.FindDebitBatchEntriesByDebitBatchId(debitBatchId, serviceHeader);

            if (includeProductDescription && debitBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(debitBatchEntries, serviceHeader);

            return debitBatchEntries;
        }

        public PageCollectionInfo<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchIdInPage(Guid debitBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var debitBatchEntries = _debitBatchAppService.FindDebitBatchEntriesByDebitBatchId(debitBatchId, text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && debitBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(debitBatchEntries.PageCollection, serviceHeader);

            return debitBatchEntries;
        }

        public List<DebitBatchEntryDTO> FindDebitBatchEntriesByCustomerId(Guid customerId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var debitBatchEntries = _debitBatchAppService.FindDebitBatchEntriesByCustomerId(customerId, serviceHeader);

            if (includeProductDescription && debitBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(debitBatchEntries, serviceHeader);

            return debitBatchEntries;
        }

        public List<BatchImportEntryWrapper> ParseDebitBatchImport(Guid debitBatchId, string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _debitBatchAppService.ParseDebitBatchImport(debitBatchId, serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);
        }

        public PageCollectionInfo<DebitBatchEntryDTO> FindQueableDebitBatchEntriesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitBatchAppService.FindQueableDebitBatchEntries(pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
