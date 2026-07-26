using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IWireTransferBatchAppService
    {
        WireTransferBatchDTO AddNewWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, ServiceHeader serviceHeader);

        bool UpdateWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, ServiceHeader serviceHeader);

        bool AuditWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        WireTransferBatchEntryDTO AddNewWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO, ServiceHeader serviceHeader);

        bool RemoveWireTransferBatchEntries(List<WireTransferBatchEntryDTO> wireTransferBatchEntryDTOs, ServiceHeader serviceHeader);

        bool UpdateWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO, ServiceHeader serviceHeader);

        bool PostWireTransferBatchEntry(Guid wireTransferBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<WireTransferBatchDTO> FindWireTransferBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<WireTransferBatchDTO> FindWireTransferBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        WireTransferBatchDTO FindWireTransferBatch(Guid wireTransferBatchId, ServiceHeader serviceHeader);

        WireTransferBatchDTO FindCachedWireTransferBatch(Guid wireTransferBatchId, ServiceHeader serviceHeader);

        WireTransferBatchEntryDTO FindWireTransferBatchEntry(Guid wireTransferBatchEntryId, ServiceHeader serviceHeader);

        List<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchId(Guid wireTransferBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchId(Guid wireTransferBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<WireTransferBatchEntryDTO> FindQueableWireTransferBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<BatchImportEntryWrapper> ParseWireTransferBatchImport(Guid wireTransferBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);
    }
}
