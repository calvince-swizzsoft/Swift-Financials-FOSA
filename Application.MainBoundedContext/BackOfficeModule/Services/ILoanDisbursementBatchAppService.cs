using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public interface ILoanDisbursementBatchAppService
    {
        LoanDisbursementBatchDTO AddNewLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, ServiceHeader serviceHeader);

        bool UpdateLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, ServiceHeader serviceHeader);

        bool AuditLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        LoanDisbursementBatchEntryDTO AddNewLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader);

        bool RemoveLoanDisbursementBatchEntries(List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntryDTOs, ServiceHeader serviceHeader);

        bool UpdateLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader);

        bool UpdateLoanDisbursementBatchEntries(Guid loanDisbursementBatchEntryId, List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntries, ServiceHeader serviceHeader);

        bool PostLoanDisbursementBatchEntry(Guid loanDisbursementBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        decimal DisburseMicroLoan(Guid alternateChannelLogId, Guid settlementChartOfAccountId, CustomerAccountDTO customerLoanAccountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<LoanDisbursementBatchDTO> FindLoanDisbursementBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<LoanDisbursementBatchDTO> FindLoanDisbursementBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LoanDisbursementBatchDTO FindLoanDisbursementBatch(Guid loanDisbursementBatchId, ServiceHeader serviceHeader);

        LoanDisbursementBatchDTO FindCachedLoanDisbursementBatch(Guid loanDisbursementBatchId, ServiceHeader serviceHeader);

        LoanDisbursementBatchEntryDTO FindLoanDisbursementBatchEntry(Guid loanDisbursementBatchEntryId, ServiceHeader serviceHeader);

        List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(Guid loanDisbursementBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(Guid loanDisbursementBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchType(int loanDisbursementBatchType, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindQueableLoanDisbursementBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByCustomerId(int loanDisbursementBatchType, Guid customerId, ServiceHeader serviceHeader);

        bool ValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(Guid loanDisbursementBatchId, Guid designationId, int transactionThresholdType, ServiceHeader serviceHeader);
    }
}
