using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IOverDeductionBatchService")]
    public interface IOverDeductionBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, AsyncCallback callback, Object state);
        OverDeductionBatchDTO EndAddOverDeductionBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, AsyncCallback callback, Object state);
        bool EndUpdateOverDeductionBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddOverDeductionBatchEntry(OverDeductionBatchEntryDTO overDeductionBatchEntryDTO, AsyncCallback callback, Object state);
        OverDeductionBatchEntryDTO EndAddOverDeductionBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveOverDeductionBatchEntries(List<OverDeductionBatchEntryDTO> overDeductionBatchEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveOverDeductionBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, AsyncCallback callback, Object state);
        bool EndAuditOverDeductionBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeOverDeductionBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseOverDeductionBatchImport(Guid overDeductionBatchId, string fileName, AsyncCallback callback, Object state);
        List<BatchImportEntryWrapper> EndParseOverDeductionBatchImport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindOverDeductionBatches(AsyncCallback callback, Object state);
        List<OverDeductionBatchDTO> EndFindOverDeductionBatches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindOverDeductionBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<OverDeductionBatchDTO> EndFindOverDeductionBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindOverDeductionBatch(Guid overDeductionBatchId, AsyncCallback callback, Object state);
        OverDeductionBatchDTO EndFindOverDeductionBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindOverDeductionBatchEntriesByOverDeductionBatchId(Guid overDeductionBatchId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<OverDeductionBatchEntryDTO> EndFindOverDeductionBatchEntriesByOverDeductionBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindOverDeductionBatchEntriesByOverDeductionBatchIdInPage(Guid overDeductionBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<OverDeductionBatchEntryDTO> EndFindOverDeductionBatchEntriesByOverDeductionBatchIdInPage(IAsyncResult result);
    }
}
