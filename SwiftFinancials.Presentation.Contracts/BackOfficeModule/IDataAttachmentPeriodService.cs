using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.BackOfficeModule
{
    [ServiceContract(Name = "IDataAttachmentPeriodService")]
    public interface IDataAttachmentPeriodService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, AsyncCallback callback, Object state);
        DataAttachmentPeriodDTO EndAddDataAttachmentPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, AsyncCallback callback, Object state);
        bool EndUpdateDataAttachmentPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCloseDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, AsyncCallback callback, Object state);
        bool EndCloseDataAttachmentPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDataAttachmentPeriods(AsyncCallback callback, Object state);
        List<DataAttachmentPeriodDTO> EndFindDataAttachmentPeriods(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDataAttachmentPeriodsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DataAttachmentPeriodDTO> EndFindDataAttachmentPeriodsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDataAttachmentPeriod(Guid dataAttachmentPeriodId, AsyncCallback callback, Object state);
        DataAttachmentPeriodDTO EndFindDataAttachmentPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDataAttachmentEntry(DataAttachmentEntryDTO dataAttachmentEntryDTO, AsyncCallback callback, Object state);
        DataAttachmentEntryDTO EndAddDataAttachmentEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveDataAttachmentEntries(List<DataAttachmentEntryDTO> dataAttachmentEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveDataAttachmentEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDataAttachmentEntriesByDataAttachmentPeriodIdAndFilterInPage(Guid dataAttachmentPeriodId, string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<DataAttachmentEntryDTO> EndFindDataAttachmentEntriesByDataAttachmentPeriodIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDataAttachmentPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DataAttachmentPeriodDTO> EndFindDataAttachmentPeriodsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCurrentDataAttachmentPeriod(AsyncCallback callback, Object state);
        DataAttachmentPeriodDTO EndFindCurrentDataAttachmentPeriod(IAsyncResult result);
    }
}
