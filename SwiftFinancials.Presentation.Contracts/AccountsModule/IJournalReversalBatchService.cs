using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IJournalReversalBatchService")]
    public interface IJournalReversalBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, AsyncCallback callback, Object state);
        JournalReversalBatchDTO EndAddJournalReversalBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, AsyncCallback callback, Object state);
        bool EndUpdateJournalReversalBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalReversalBatchEntry(JournalReversalBatchEntryDTO journalReversalBatchEntryDTO, AsyncCallback callback, Object state);
        JournalReversalBatchEntryDTO EndAddJournalReversalBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveJournalReversalBatchEntries(List<JournalReversalBatchEntryDTO> journalReversalBatchEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveJournalReversalBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, AsyncCallback callback, Object state);
        bool EndAuditJournalReversalBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeJournalReversalBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateJournalReversalBatchEntries(Guid journalReversalBatchId, List<JournalReversalBatchEntryDTO> journalReversalBatchEntries, AsyncCallback callback, Object state);
        bool EndUpdateJournalReversalBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalReversalBatches(AsyncCallback callback, Object state);
        List<JournalReversalBatchDTO> EndFindJournalReversalBatches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalReversalBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalReversalBatchDTO> EndFindJournalReversalBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalReversalBatch(Guid journalReversalBatchId, AsyncCallback callback, Object state);
        JournalReversalBatchDTO EndFindJournalReversalBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalReversalBatchEntriesByJournalReversalBatchId(Guid journalReversalBatchId, AsyncCallback callback, Object state);
        List<JournalReversalBatchEntryDTO> EndFindJournalReversalBatchEntriesByJournalReversalBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalReversalBatchEntriesByJournalReversalBatchIdInPage(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalReversalBatchEntryDTO> EndFindJournalReversalBatchEntriesByJournalReversalBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalEntriesByJournalReversalBatchIdInPage(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalEntryDTO> EndFindJournalEntriesByJournalReversalBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueableJournalReversalBatchEntriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalReversalBatchEntryDTO> EndFindQueableJournalReversalBatchEntriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostJournalReversalBatchEntry(Guid journalReversalBatchEntryId, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPostJournalReversalBatchEntry(IAsyncResult result);
    }
}
