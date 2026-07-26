using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.BackOfficeModule
{
    [ServiceContract(Name = "ILoanDisbursementBatchService")]
    public interface ILoanDisbursementBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, AsyncCallback callback, Object state);
        LoanDisbursementBatchDTO EndAddLoanDisbursementBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, AsyncCallback callback, Object state);
        bool EndUpdateLoanDisbursementBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, AsyncCallback callback, Object state);
        bool EndAuditLoanDisbursementBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeLoanDisbursementBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, AsyncCallback callback, Object state);
        LoanDisbursementBatchEntryDTO EndAddLoanDisbursementBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveLoanDisbursementBatchEntries(List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveLoanDisbursementBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, AsyncCallback callback, Object state);
        bool EndUpdateLoanDisbursementBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanDisbursementBatchEntries(Guid loanDisbursementBatchId, List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntries, AsyncCallback callback, Object state);
        bool EndUpdateLoanDisbursementBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostLoanDisbursementBatchEntry(Guid loanDisbursementBatchEntryId, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPostLoanDisbursementBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginDisburseMicroLoan(Guid alternateChannelLogId, Guid settlementChartOfAccountId, CustomerAccountDTO customerLoanAccountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        decimal EndDisburseMicroLoan(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanDisbursementBatches(AsyncCallback callback, Object state);
        List<LoanDisbursementBatchDTO> EndFindLoanDisbursementBatches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanDisbursementBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanDisbursementBatchDTO> EndFindLoanDisbursementBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanDisbursementBatch(Guid loanDisbursementBatchId, AsyncCallback callback, Object state);
        LoanDisbursementBatchDTO EndFindLoanDisbursementBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(Guid loanDisbursementBatchId, AsyncCallback callback, Object state);
        List<LoanDisbursementBatchEntryDTO> EndFindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanDisbursementBatchEntriesByLoanDisbursementBatchIdInPage(Guid loanDisbursementBatchId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanDisbursementBatchEntryDTO> EndFindLoanDisbursementBatchEntriesByLoanDisbursementBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanDisbursementBatchEntriesByLoanDisbursementBatchTypeInPage(int loanDisbursementBatchType, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanDisbursementBatchEntryDTO> EndFindLoanDisbursementBatchEntriesByLoanDisbursementBatchTypeInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanDisbursementBatchEntriesByCustomerId(int loanDisbursementBatchType, Guid customerId, AsyncCallback callback, Object state);
        List<LoanDisbursementBatchEntryDTO> EndFindLoanDisbursementBatchEntriesByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueableLoanDisbursementBatchEntriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanDisbursementBatchEntryDTO> EndFindQueableLoanDisbursementBatchEntriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(Guid loanDisbursementBatchId, Guid designationId, int transactionThresholdType, AsyncCallback callback, Object state);
        bool EndValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(IAsyncResult result);
    }
}
