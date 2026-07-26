using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IAlternateChannelReconciliationPeriodService")]
    public interface IAlternateChannelReconciliationPeriodService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, AsyncCallback callback, Object state);
        AlternateChannelReconciliationPeriodDTO EndAddAlternateChannelReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, AsyncCallback callback, Object state);
        bool EndUpdateAlternateChannelReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCloseAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, int alternateChannelReconciliationPeriodAuthOption, AsyncCallback callback, Object state);
        bool EndCloseAlternateChannelReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelReconciliationPeriods(AsyncCallback callback, Object state);
        List<AlternateChannelReconciliationPeriodDTO> EndFindAlternateChannelReconciliationPeriods(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelReconciliationPeriodsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> EndFindAlternateChannelReconciliationPeriodsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelReconciliationPeriod(Guid alternateChannelReconciliationPeriodId, AsyncCallback callback, Object state);
        AlternateChannelReconciliationPeriodDTO EndFindAlternateChannelReconciliationPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAlternateChannelReconciliationEntry(AlternateChannelReconciliationEntryDTO alternateChannelReconciliationEntryDTO, AsyncCallback callback, Object state);
        AlternateChannelReconciliationEntryDTO EndAddAlternateChannelReconciliationEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveAlternateChannelReconciliationEntries(List<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveAlternateChannelReconciliationEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodIdAndFilterInPage(Guid alternateChannelReconciliationPeriodId, int status, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelReconciliationEntryDTO> EndFindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelReconciliationPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> EndFindAlternateChannelReconciliationPeriodsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseAlternateChannelReconciliationImport(Guid alternateChannelReconciliationPeriodId, string fileName, AsyncCallback callback, Object state);
        List<BatchImportEntryWrapper> EndParseAlternateChannelReconciliationImport(IAsyncResult result);
    }
}
