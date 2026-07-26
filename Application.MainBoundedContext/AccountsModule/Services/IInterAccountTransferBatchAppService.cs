using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IInterAccountTransferBatchAppService
    {
        InterAccountTransferBatchDTO AddNewInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, ServiceHeader serviceHeader);

        bool UpdateInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, ServiceHeader serviceHeader);

        bool UpdateDynamicCharges(Guid interAccountTransferBatchId, List<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader);

        InterAccountTransferBatchEntryDTO AddNewInterAccountTransferBatchEntry(InterAccountTransferBatchEntryDTO interAccountTransferBatchEntryDTO, ServiceHeader serviceHeader);

        bool RemoveInterAccountTransferBatchEntries(List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryDTOs, ServiceHeader serviceHeader);

        bool AuditInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool UpdateInterAccountTransferBatchEntryCollection(Guid interAccountTransferBatchId, List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryCollection, ServiceHeader serviceHeader);

        List<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        InterAccountTransferBatchDTO FindInterAccountTransferBatch(Guid interAccountTransferBatchId, ServiceHeader serviceHeader);

        List<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<DynamicChargeDTO> FindDynamicCharges(Guid interAccountTransferBatchId, ServiceHeader serviceHeader);

        List<DynamicChargeDTO> FindCachedDynamicCharges(Guid interAccountTransferBatchId, ServiceHeader serviceHeader);
    }
}
