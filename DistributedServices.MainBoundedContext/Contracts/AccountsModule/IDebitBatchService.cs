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
    public interface IDebitBatchService
    {
        #region Debit Batch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DebitBatchDTO AddDebitBatch(DebitBatchDTO debitBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDebitBatch(DebitBatchDTO debitBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostDebitBatchEntry(Guid debitBatchEntryId, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DebitBatchEntryDTO AddDebitBatchEntry(DebitBatchEntryDTO debitBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveDebitBatchEntries(List<DebitBatchEntryDTO> debitBatchEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DebitBatchDTO> FindDebitBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DebitBatchDTO> FindDebitBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DebitBatchDTO FindDebitBatch(Guid debitBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchId(Guid debitBatchId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchIdInPage(Guid debitBatchId, string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DebitBatchEntryDTO> FindDebitBatchEntriesByCustomerId(Guid customerId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BatchImportEntryWrapper> ParseDebitBatchImport(Guid debitBatchId, string fileName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DebitBatchDTO> FindDebitBatchesByFilterInPage(string text, int pageIndex, int pageSize);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DebitBatchEntryDTO> FindQueableDebitBatchEntriesInPage(int pageIndex, int pageSize);

        #endregion
    }
}
