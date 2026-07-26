using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IWireTransferBatchService")]
    public interface IWireTransferBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, AsyncCallback callback, Object state);
        WireTransferBatchDTO EndAddWireTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, AsyncCallback callback, Object state);
        bool EndUpdateWireTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, AsyncCallback callback, Object state);
        bool EndAuditWireTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeWireTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO, AsyncCallback callback, Object state);
        WireTransferBatchEntryDTO EndAddWireTransferBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveWireTransferBatchEntries(List<WireTransferBatchEntryDTO> wireTransferBatchEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveWireTransferBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO, AsyncCallback callback, Object state);
        bool EndUpdateWireTransferBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferBatches(AsyncCallback callback, Object state);
        List<WireTransferBatchDTO> EndFindWireTransferBatches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WireTransferBatchDTO> EndFindWireTransferBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferBatch(Guid wireTransferBatchId, AsyncCallback callback, Object state);
        WireTransferBatchDTO EndFindWireTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferBatchEntriesByWireTransferBatchId(Guid wireTransferBatchId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<WireTransferBatchEntryDTO> EndFindWireTransferBatchEntriesByWireTransferBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferBatchEntriesByWireTransferBatchIdInPage(Guid wireTransferBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<WireTransferBatchEntryDTO> EndFindWireTransferBatchEntriesByWireTransferBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueableWireTransferBatchEntriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WireTransferBatchEntryDTO> EndFindQueableWireTransferBatchEntriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseWireTransferBatchImport(Guid wireTransferBatchId, string fileName, AsyncCallback callback, Object state);
        List<BatchImportEntryWrapper> EndParseWireTransferBatchImport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostWireTransferBatchEntry(Guid wireTransferBatchEntryId, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPostWireTransferBatchEntry(IAsyncResult result);
    }
}
