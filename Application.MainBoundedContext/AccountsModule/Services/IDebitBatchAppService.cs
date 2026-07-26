using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IDebitBatchAppService
    {
        DebitBatchDTO AddNewDebitBatch(DebitBatchDTO debitBatchDTO, ServiceHeader serviceHeader);

        bool UpdateDebitBatch(DebitBatchDTO debitBatchDTO, ServiceHeader serviceHeader);

        bool AuditDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool PostDebitBatchEntry(Guid debitBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        DebitBatchEntryDTO AddNewDebitBatchEntry(DebitBatchEntryDTO debitBatchEntryDTO, ServiceHeader serviceHeader);

        bool RemoveDebitBatchEntries(List<DebitBatchEntryDTO> debitBatchEntryDTOs, ServiceHeader serviceHeader);

        List<DebitBatchDTO> FindDebitBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<DebitBatchDTO> FindDebitBatches(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DebitBatchDTO> FindDebitBatches(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DebitBatchDTO> FindDebitBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DebitBatchDTO FindDebitBatch(Guid debitBatchId, ServiceHeader serviceHeader);

        List<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchId(Guid debitBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchId(Guid debitBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<DebitBatchEntryDTO> FindDebitBatchEntriesByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        PageCollectionInfo<DebitBatchEntryDTO> FindQueableDebitBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<BatchImportEntryWrapper> ParseDebitBatchImport(Guid debitBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);
    }
}
