using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ICreditBatchService")]
    public interface ICreditBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCreditBatch(CreditBatchDTO creditBatchDTO, AsyncCallback callback, Object state);
        CreditBatchDTO EndAddCreditBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCreditBatch(CreditBatchDTO creditBatchDTO, AsyncCallback callback, Object state);
        bool EndUpdateCreditBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, AsyncCallback callback, Object state);
        bool EndAuditCreditBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeCreditBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchCreditBatchDiscrepancyByGeneralLedgerAccount(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, Guid chartOfAccountId, int moduleNavigationItemCode, int discrepancyAuthOption, AsyncCallback callback, Object state);
        bool EndMatchCreditBatchDiscrepancyByGeneralLedgerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchCreditBatchDiscrepancyByCustomerAccount(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, CustomerAccountDTO customerAccountDTO, int discrepancyAuthOption, AsyncCallback callback, Object state);
        bool EndMatchCreditBatchDiscrepancyByCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchCreditBatchDiscrepanciesByCustomerAccount(List<CreditBatchDiscrepancyDTO> creditBatchDiscrepancyDTOs, CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        bool EndMatchCreditBatchDiscrepanciesByCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO, AsyncCallback callback, Object state);
        CreditBatchEntryDTO EndAddCreditBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveCreditBatchEntries(List<CreditBatchEntryDTO> creditBatchEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveCreditBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO, AsyncCallback callback, Object state);
        bool EndUpdateCreditBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostCreditBatchEntry(Guid creditBatchEntryId, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPostCreditBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatches(AsyncCallback callback, Object state);
        List<CreditBatchDTO> EndFindCreditBatches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchDTO> EndFindCreditBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatch(Guid creditBatchId, AsyncCallback callback, Object state);
        CreditBatchDTO EndFindCreditBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchEntriesByCreditBatchId(Guid creditBatchId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<CreditBatchEntryDTO> EndFindCreditBatchEntriesByCreditBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchEntriesByCreditBatchIdInPage(Guid creditBatchId, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchEntryDTO> EndFindCreditBatchEntriesByCreditBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchDiscrepanciesByCreditBatchIdInPage(Guid creditBatchId, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchDiscrepancyDTO> EndFindCreditBatchDiscrepanciesByCreditBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchDiscrepanciesByCreditBatchTypeInPage(int creditBatchType, int status, int productCode, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchDiscrepancyDTO> EndFindCreditBatchDiscrepanciesByCreditBatchTypeInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchDiscrepanciesInPage(int status, DateTime startDate, DateTime endDate, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchDiscrepancyDTO> EndFindCreditBatchDiscrepanciesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchEntriesByCreditBatchTypeInPage(int creditBatchType, DateTime startDate, DateTime endDate, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchEntryDTO> EndFindCreditBatchEntriesByCreditBatchTypeInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchEntriesByCustomerId(int creditBatchType, Guid customerId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<CreditBatchEntryDTO> EndFindCreditBatchEntriesByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanAppraisalCreditBatchEntriesByCustomerId(Guid customerId, Guid loanProductId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<CreditBatchEntryDTO> EndFindLoanAppraisalCreditBatchEntriesByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseCreditBatchImport(Guid creditBatchId, string fileName, AsyncCallback callback, Object state);
        List<BatchImportEntryWrapper> EndParseCreditBatchImport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLastCreditBatchEntryByCustomerAccountId(Guid customerAccountId, int creditBatchType, AsyncCallback callback, Object state);
        CreditBatchEntryDTO EndFindLastCreditBatchEntryByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueableCreditBatchEntriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchEntryDTO> EndFindQueableCreditBatchEntriesInPage(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditBatchesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditBatchDTO> EndFindCreditBatchesByFilterInPage(IAsyncResult result);
    }
}
