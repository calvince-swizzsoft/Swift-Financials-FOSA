using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IOverDeductionBatchAppService
    {
        OverDeductionBatchDTO AddNewOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, ServiceHeader serviceHeader);

        bool UpdateOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, ServiceHeader serviceHeader);

        OverDeductionBatchEntryDTO AddNewOverDeductionBatchEntry(OverDeductionBatchEntryDTO overDeductionBatchEntryDTO, ServiceHeader serviceHeader);

        bool RemoveOverDeductionBatchEntries(List<OverDeductionBatchEntryDTO> overDeductionBatchEntryDTOs, ServiceHeader serviceHeader);

        bool AuditOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<BatchImportEntryWrapper> ParseOverDeductionBatchImport(Guid overDeductionBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);

        List<OverDeductionBatchDTO> FindOverDeductionBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<OverDeductionBatchDTO> FindOverDeductionBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        OverDeductionBatchDTO FindOverDeductionBatch(Guid overDeductionBatchId, ServiceHeader serviceHeader);

        List<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchId(Guid overDeductionBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchId(Guid overDeductionBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
