using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IOverDeductionBatchService
    {
        #region Over Deduction Batch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        OverDeductionBatchDTO AddOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        OverDeductionBatchEntryDTO AddOverDeductionBatchEntry(OverDeductionBatchEntryDTO overDeductionBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveOverDeductionBatchEntries(List<OverDeductionBatchEntryDTO> overDeductionBatchEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BatchImportEntryWrapper> ParseOverDeductionBatchImport(Guid overDeductionBatchId, string fileName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<OverDeductionBatchDTO> FindOverDeductionBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<OverDeductionBatchDTO> FindOverDeductionBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        OverDeductionBatchDTO FindOverDeductionBatch(Guid overDeductionBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchId(Guid overDeductionBatchId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchIdInPage(Guid overDeductionBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription);

        #endregion
    }
}
