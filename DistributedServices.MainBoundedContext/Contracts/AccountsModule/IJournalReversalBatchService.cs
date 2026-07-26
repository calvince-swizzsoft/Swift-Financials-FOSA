using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IJournalReversalBatchService
    {
        #region Journal Reversal Batch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalReversalBatchDTO AddJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalReversalBatchEntryDTO AddJournalReversalBatchEntry(JournalReversalBatchEntryDTO journalReversalBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveJournalReversalBatchEntries(List<JournalReversalBatchEntryDTO> journalReversalBatchEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateJournalReversalBatchEntries(Guid journalReversalBatchId, List<JournalReversalBatchEntryDTO> journalReversalBatchEntries);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<JournalReversalBatchDTO> FindJournalReversalBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalReversalBatchDTO> FindJournalReversalBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalReversalBatchDTO FindJournalReversalBatch(Guid journalReversalBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchId(Guid journalReversalBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchIdInPage(Guid journalReversalBatchId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalEntryDTO> FindJournalEntriesByJournalReversalBatchIdInPage(Guid journalReversalBatchId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalReversalBatchEntryDTO> FindQueableJournalReversalBatchEntriesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostJournalReversalBatchEntry(Guid journalReversalBatchEntryId, int moduleNavigationItemCode);

        #endregion
    }
}
