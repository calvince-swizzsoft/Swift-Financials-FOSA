using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IWireTransferBatchService
    {
        #region Wire Transfer Batch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WireTransferBatchDTO AddWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostWireTransferBatchEntry(Guid wireTransferBatchEntryId, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WireTransferBatchEntryDTO AddWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveWireTransferBatchEntries(List<WireTransferBatchEntryDTO> wireTransferBatchEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<WireTransferBatchDTO> FindWireTransferBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WireTransferBatchDTO> FindWireTransferBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WireTransferBatchDTO FindWireTransferBatch(Guid wireTransferBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchId(Guid wireTransferBatchId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchIdInPage(Guid wireTransferBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BatchImportEntryWrapper> ParseWireTransferBatchImport(Guid wireTransferBatchId, string fileName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WireTransferBatchEntryDTO> FindQueableWireTransferBatchEntriesInPage(int pageIndex, int pageSize);

        #endregion
    }
}
