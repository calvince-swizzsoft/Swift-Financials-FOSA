using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IJournalReversalBatchAppService
    {
        JournalReversalBatchDTO AddNewJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, ServiceHeader serviceHeader);

        bool UpdateJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, ServiceHeader serviceHeader);

        JournalReversalBatchEntryDTO AddNewJournalReversalBatchEntry(JournalReversalBatchEntryDTO journalReversalBatchEntryDTO, ServiceHeader serviceHeader);

        bool RemoveJournalReversalBatchEntries(List<JournalReversalBatchEntryDTO> journalReversalBatchEntryDTOs, ServiceHeader serviceHeader);

        bool AuditJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool UpdateJournalReversalBatchEntries(Guid journalReversalBatchId, List<JournalReversalBatchEntryDTO> journalReversalBatchEntries, ServiceHeader serviceHeader);

        List<JournalReversalBatchDTO> FindJournalReversalBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<JournalReversalBatchDTO> FindJournalReversalBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        JournalReversalBatchDTO FindJournalReversalBatch(Guid journalReversalBatchId, ServiceHeader serviceHeader);

        JournalReversalBatchEntryDTO FindJournalReversalBatchEntry(Guid journalReversalBatchEntryId, ServiceHeader serviceHeader);

        List<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchId(Guid journalReversalBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchId(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalEntryDTO> FindJournalEntriesByJournalReversalBatchId(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalReversalBatchEntryDTO> FindQueableJournalReversalBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool PostJournalReversalBatchEntry(Guid journalReversalBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader);
    }
}
