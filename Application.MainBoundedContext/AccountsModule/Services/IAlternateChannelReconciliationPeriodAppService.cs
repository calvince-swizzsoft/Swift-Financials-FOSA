using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IAlternateChannelReconciliationPeriodAppService
    {
        AlternateChannelReconciliationPeriodDTO AddNewAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, ServiceHeader serviceHeader);

        bool UpdateAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, ServiceHeader serviceHeader);

        bool CloseAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, int alternateChannelReconciliationPeriodAuthOption, ServiceHeader serviceHeader);

        AlternateChannelReconciliationEntryDTO AddNewAlternateChannelReconciliationEntry(AlternateChannelReconciliationEntryDTO alternateChannelReconciliationEntryDTO, ServiceHeader serviceHeader);

        bool RemoveAlternateChannelReconciliationEntries(List<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntryDTOs, ServiceHeader serviceHeader);

        List<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        AlternateChannelReconciliationPeriodDTO FindAlternateChannelReconciliationPeriod(Guid alternateChannelReconciliationPeriodId, ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelReconciliationEntryDTO> FindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodId(Guid alternateChannelReconciliationPeriodId, int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<BatchImportEntryWrapper> ParseAlternateChannelReconciliationImport(Guid alternateChannelReconciliationPeriodId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);
    }
}
