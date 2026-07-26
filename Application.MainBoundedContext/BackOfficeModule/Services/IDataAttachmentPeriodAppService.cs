using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public interface IDataAttachmentPeriodAppService
    {
        DataAttachmentPeriodDTO AddNewDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader);

        bool UpdateDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader);

        bool CloseDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader);

        DataAttachmentEntryDTO AddNewDataAttachmentEntry(DataAttachmentEntryDTO dataAttachmentEntryDTO, ServiceHeader serviceHeader);

        bool RemoveDataAttachmentEntries(List<DataAttachmentEntryDTO> dataAttachmentEntryDTOs, ServiceHeader serviceHeader);

        List<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(ServiceHeader serviceHeader);

        PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DataAttachmentPeriodDTO FindDataAttachmentPeriod(Guid dataAttachmentPeriodId, ServiceHeader serviceHeader);

        List<DataAttachmentEntryDTO> FindDataAttachmentEntriesByDataAttachmentPeriodIdAndCustomerAccountId(Guid dataAttachmentPeriodId, Guid customerAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<DataAttachmentEntryDTO> FindDataAttachmentEntriesByDataAttachmentPeriodId(Guid dataAttachmentPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DataAttachmentPeriodDTO FindCurrentDataAttachmentPeriod(ServiceHeader serviceHeader);

        DataAttachmentPeriodDTO FindCachedCurrentDataAttachmentPeriod(ServiceHeader serviceHeader);
    }
}
