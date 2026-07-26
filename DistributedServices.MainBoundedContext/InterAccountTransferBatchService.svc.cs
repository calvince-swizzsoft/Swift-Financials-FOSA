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
    public class InterAccountTransferBatchService : IInterAccountTransferBatchService
    {
        private readonly IInterAccountTransferBatchAppService _interAccountTransferBatchAppService;

        public InterAccountTransferBatchService(
           IInterAccountTransferBatchAppService interAccountTransferBatchAppService)
        {
            Guard.ArgumentNotNull(interAccountTransferBatchAppService, nameof(interAccountTransferBatchAppService));

            _interAccountTransferBatchAppService = interAccountTransferBatchAppService;
        }

        #region Inter-Account Transfer Batch

        public InterAccountTransferBatchDTO AddInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.AddNewInterAccountTransferBatch(interAccountTransferBatchDTO, serviceHeader);
        }

        public bool UpdateInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.UpdateInterAccountTransferBatch(interAccountTransferBatchDTO, serviceHeader);
        }

        public bool UpdateDynamicChargesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, List<DynamicChargeDTO> dynamicCharges)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.UpdateDynamicCharges(interAccountTransferBatchId, dynamicCharges, serviceHeader);
        }

        public InterAccountTransferBatchEntryDTO AddInterAccountTransferBatchEntry(InterAccountTransferBatchEntryDTO interAccountTransferBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.AddNewInterAccountTransferBatchEntry(interAccountTransferBatchEntryDTO, serviceHeader);
        }

        public bool RemoveInterAccountTransferBatchEntries(List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.RemoveInterAccountTransferBatchEntries(interAccountTransferBatchEntryDTOs, serviceHeader);
        }

        public bool AuditInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.AuditInterAccountTransferBatch(interAccountTransferBatchDTO, batchAuthOption, serviceHeader);
        }

        public bool AuthorizeInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.AuthorizeInterAccountTransferBatch(interAccountTransferBatchDTO, batchAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool UpdateInterAccountTransferBatchEntryCollection(Guid interAccountTransferBatchId, List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.UpdateInterAccountTransferBatchEntryCollection(interAccountTransferBatchId, interAccountTransferBatchEntryCollection, serviceHeader);
        }

        public List<InterAccountTransferBatchDTO> FindInterAccountTransferBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindInterAccountTransferBatches(serviceHeader);
        }

        public PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatchesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindInterAccountTransferBatches(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatchesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindInterAccountTransferBatches(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindInterAccountTransferBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public InterAccountTransferBatchDTO FindInterAccountTransferBatch(Guid interAccountTransferBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindInterAccountTransferBatch(interAccountTransferBatchId, serviceHeader);
        }

        public List<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(Guid interAccountTransferBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(interAccountTransferBatchId, serviceHeader);
        }

        public PageCollectionInfo<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchIdInPage(Guid interAccountTransferBatchId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(interAccountTransferBatchId, text, pageIndex, pageSize, serviceHeader);
        }

        public List<DynamicChargeDTO> FindDynamicChargesByInterAccountTransferBatchId(Guid interAccountTransferBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _interAccountTransferBatchAppService.FindDynamicCharges(interAccountTransferBatchId, serviceHeader);
        }

        #endregion
    }
}
