using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IAlternateChannelReconciliationPeriodService
    {
        #region Alternate Channel Reconciliation Period

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AlternateChannelReconciliationPeriodDTO AddAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CloseAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, int alternateChannelReconciliationPeriodAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriodsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AlternateChannelReconciliationPeriodDTO FindAlternateChannelReconciliationPeriod(Guid alternateChannelReconciliationPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AlternateChannelReconciliationEntryDTO AddAlternateChannelReconciliationEntry(AlternateChannelReconciliationEntryDTO alternateChannelReconciliationEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveAlternateChannelReconciliationEntries(List<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelReconciliationEntryDTO> FindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodIdAndFilterInPage(Guid alternateChannelReconciliationPeriodId, int status, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BatchImportEntryWrapper> ParseAlternateChannelReconciliationImport(Guid alternateChannelReconciliationPeriodId, string fileName);

        #endregion
    }
}
