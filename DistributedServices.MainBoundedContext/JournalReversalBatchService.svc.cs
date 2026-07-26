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
    public class JournalReversalBatchService : IJournalReversalBatchService
    {
        private readonly IJournalReversalBatchAppService _journalReversalBatchAppService;

        public JournalReversalBatchService(
            IJournalReversalBatchAppService journalReversalBatchAppService)
        {
            Guard.ArgumentNotNull(journalReversalBatchAppService, nameof(journalReversalBatchAppService));

            _journalReversalBatchAppService = journalReversalBatchAppService;
        }

        #region Over Deduction Batch

        public JournalReversalBatchDTO AddJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.AddNewJournalReversalBatch(journalReversalBatchDTO, serviceHeader);
        }

        public bool UpdateJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.UpdateJournalReversalBatch(journalReversalBatchDTO, serviceHeader);
        }

        public JournalReversalBatchEntryDTO AddJournalReversalBatchEntry(JournalReversalBatchEntryDTO journalReversalBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.AddNewJournalReversalBatchEntry(journalReversalBatchEntryDTO, serviceHeader);
        }

        public bool RemoveJournalReversalBatchEntries(List<JournalReversalBatchEntryDTO> journalReversalBatchEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.RemoveJournalReversalBatchEntries(journalReversalBatchEntryDTOs, serviceHeader);
        }

        public bool AuditJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.AuditJournalReversalBatch(journalReversalBatchDTO, batchAuthOption, serviceHeader);
        }

        public bool AuthorizeJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.AuthorizeJournalReversalBatch(journalReversalBatchDTO, batchAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool UpdateJournalReversalBatchEntries(Guid journalReversalBatchId, List<JournalReversalBatchEntryDTO> journalReversalBatchEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.UpdateJournalReversalBatchEntries(journalReversalBatchId, journalReversalBatchEntries, serviceHeader);
        }

        public List<JournalReversalBatchDTO> FindJournalReversalBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.FindJournalReversalBatches(serviceHeader);
        }

        public PageCollectionInfo<JournalReversalBatchDTO> FindJournalReversalBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.FindJournalReversalBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public JournalReversalBatchDTO FindJournalReversalBatch(Guid journalReversalBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.FindJournalReversalBatch(journalReversalBatchId, serviceHeader);
        }

        public List<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchId(Guid journalReversalBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.FindJournalReversalBatchEntriesByJournalReversalBatchId(journalReversalBatchId, serviceHeader);
        }

        public PageCollectionInfo<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchIdInPage(Guid journalReversalBatchId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.FindJournalReversalBatchEntriesByJournalReversalBatchId(journalReversalBatchId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<JournalEntryDTO> FindJournalEntriesByJournalReversalBatchIdInPage(Guid journalReversalBatchId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.FindJournalEntriesByJournalReversalBatchId(journalReversalBatchId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<JournalReversalBatchEntryDTO> FindQueableJournalReversalBatchEntriesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.FindQueableJournalReversalBatchEntries(pageIndex, pageSize, serviceHeader);
        }

        public bool PostJournalReversalBatchEntry(Guid journalReversalBatchEntryId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalReversalBatchAppService.PostJournalReversalBatchEntry(journalReversalBatchEntryId, moduleNavigationItemCode, serviceHeader);
        }

        #endregion
    }
}
