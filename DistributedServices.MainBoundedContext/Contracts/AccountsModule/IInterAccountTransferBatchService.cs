using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IInterAccountTransferBatchService
    {
        #region Inter-Account Transfer Batch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InterAccountTransferBatchDTO AddInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDynamicChargesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, List<DynamicChargeDTO> dynamicCharges);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InterAccountTransferBatchEntryDTO AddInterAccountTransferBatchEntry(InterAccountTransferBatchEntryDTO interAccountTransferBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveInterAccountTransferBatchEntries(List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateInterAccountTransferBatchEntryCollection(Guid interAccountTransferBatchId, List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<InterAccountTransferBatchDTO> FindInterAccountTransferBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatchesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatchesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InterAccountTransferBatchDTO FindInterAccountTransferBatch(Guid interAccountTransferBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(Guid interAccountTransferBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchIdInPage(Guid interAccountTransferBatchId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DynamicChargeDTO> FindDynamicChargesByInterAccountTransferBatchId(Guid interAccountTransferBatchId);

        #endregion
    }
}
