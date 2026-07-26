using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IDataAttachmentPeriodService
    {
        #region Data Attachment Period

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DataAttachmentPeriodDTO AddDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CloseDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DataAttachmentPeriodDTO> FindDataAttachmentPeriods();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriodsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DataAttachmentPeriodDTO FindDataAttachmentPeriod(Guid dataAttachmentPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DataAttachmentEntryDTO AddDataAttachmentEntry(DataAttachmentEntryDTO dataAttachmentEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveDataAttachmentEntries(List<DataAttachmentEntryDTO> dataAttachmentEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DataAttachmentEntryDTO> FindDataAttachmentEntriesByDataAttachmentPeriodIdAndFilterInPage(Guid dataAttachmentPeriodId, string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DataAttachmentPeriodDTO FindCurrentDataAttachmentPeriod();

        #endregion
    }
}
