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
    public interface ICreditBatchService
    {
        #region Credit Batch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CreditBatchDTO AddCreditBatch(CreditBatchDTO creditBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCreditBatch(CreditBatchDTO creditBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MatchCreditBatchDiscrepancyByGeneralLedgerAccount(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, Guid chartOfAccountId, int moduleNavigationItemCode, int discrepancyAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MatchCreditBatchDiscrepancyByCustomerAccount(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, CustomerAccountDTO customerAccountDTO, int discrepancyAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MatchCreditBatchDiscrepanciesByCustomerAccount(List<CreditBatchDiscrepancyDTO> creditBatchDiscrepancyDTOs, CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CreditBatchEntryDTO AddCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveCreditBatchEntries(List<CreditBatchEntryDTO> creditBatchEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostCreditBatchEntry(Guid creditBatchEntryId, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CreditBatchDTO> FindCreditBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchDTO> FindCreditBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CreditBatchDTO FindCreditBatch(Guid creditBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchId(Guid creditBatchId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchIdInPage(Guid creditBatchId, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesByCreditBatchIdInPage(Guid creditBatchId, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesByCreditBatchTypeInPage(int creditBatchType, int status, int productCode, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesInPage(int status, DateTime startDate, DateTime endDate, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchTypeInPage(int creditBatchType, DateTime startDate, DateTime endDate, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CreditBatchEntryDTO> FindCreditBatchEntriesByCustomerId(int creditBatchType, Guid customerId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CreditBatchEntryDTO> FindLoanAppraisalCreditBatchEntriesByCustomerId(Guid customerId, Guid loanProductId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BatchImportEntryWrapper> ParseCreditBatchImport(Guid creditBatchId, string fileName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CreditBatchEntryDTO FindLastCreditBatchEntryByCustomerAccountId(Guid customerAccountId, int creditBatchType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchDTO> FindCreditBatchesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditBatchEntryDTO> FindQueableCreditBatchEntriesInPage(int pageIndex, int pageSize);

        #endregion
    }
}
