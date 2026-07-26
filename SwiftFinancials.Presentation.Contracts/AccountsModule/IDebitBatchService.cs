using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IDebitBatchService")]
    public interface IDebitBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDebitBatch(DebitBatchDTO debitBatchDTO, AsyncCallback callback, Object state);
        DebitBatchDTO EndAddDebitBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDebitBatch(DebitBatchDTO debitBatchDTO, AsyncCallback callback, Object state);
        bool EndUpdateDebitBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, AsyncCallback callback, Object state);
        bool EndAuditDebitBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeDebitBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostDebitBatchEntry(Guid debitBatchEntryId, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPostDebitBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDebitBatchEntry(DebitBatchEntryDTO debitBatchEntryDTO, AsyncCallback callback, Object state);
        DebitBatchEntryDTO EndAddDebitBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveDebitBatchEntries(List<DebitBatchEntryDTO> debitBatchEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveDebitBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitBatches(AsyncCallback callback, Object state);
        List<DebitBatchDTO> EndFindDebitBatches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DebitBatchDTO> EndFindDebitBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitBatch(Guid debitBatchId, AsyncCallback callback, Object state);
        DebitBatchDTO EndFindDebitBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitBatchEntriesByDebitBatchId(Guid debitBatchId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<DebitBatchEntryDTO> EndFindDebitBatchEntriesByDebitBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitBatchEntriesByDebitBatchIdInPage(Guid debitBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<DebitBatchEntryDTO> EndFindDebitBatchEntriesByDebitBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitBatchEntriesByCustomerId(Guid customerId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<DebitBatchEntryDTO> EndFindDebitBatchEntriesByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseDebitBatchImport(Guid debitBatchId, string fileName, AsyncCallback callback, Object state);
        List<BatchImportEntryWrapper> EndParseDebitBatchImport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueableDebitBatchEntriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DebitBatchEntryDTO> EndFindQueableDebitBatchEntriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitBatchesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DebitBatchDTO> EndFindDebitBatchesByFilterInPage(IAsyncResult result);
    }
}
