using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILoanDisbursementBatchService
    {
        #region Loan Disbursement Batch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanDisbursementBatchDTO AddLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanDisbursementBatchEntryDTO AddLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveLoanDisbursementBatchEntries(List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanDisbursementBatchEntries(Guid loanDisbursementBatchId, List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntries);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostLoanDisbursementBatchEntry(Guid loanDisbursementBatchEntryId, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        decimal DisburseMicroLoan(Guid alternateChannelLogId, Guid settlementChartOfAccountId, CustomerAccountDTO customerLoanAccountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanDisbursementBatchDTO> FindLoanDisbursementBatches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanDisbursementBatchDTO> FindLoanDisbursementBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanDisbursementBatchDTO FindLoanDisbursementBatch(Guid loanDisbursementBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(Guid loanDisbursementBatchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchIdInPage(Guid loanDisbursementBatchId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchTypeInPage(int loanDisbursementBatchType, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByCustomerId(int loanDisbursementBatchType, Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindQueableLoanDisbursementBatchEntriesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(Guid loanDisbursementBatchId, Guid designationId, int transactionThresholdType);

        #endregion
    }
}
