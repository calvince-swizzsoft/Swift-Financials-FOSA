using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg
{
    public static class LoanDisbursementBatchFactory
    {
        public static LoanDisbursementBatch CreateLoanDisbursementBatch(Guid branchId, Guid? dataAttachmentPeriodId, int type, int loanProductCategory, string reference, int priority)
        {
            var loanDisbursementBatch = new LoanDisbursementBatch();

            loanDisbursementBatch.GenerateNewIdentity();

            loanDisbursementBatch.BranchId = branchId;

            loanDisbursementBatch.DataAttachmentPeriodId = (dataAttachmentPeriodId != null && dataAttachmentPeriodId != Guid.Empty) ? dataAttachmentPeriodId : null;

            loanDisbursementBatch.Type = (byte)type;

            loanDisbursementBatch.LoanProductCategory = (byte)loanProductCategory;

            loanDisbursementBatch.Reference = reference;

            loanDisbursementBatch.Priority = (byte)priority;

            loanDisbursementBatch.CreatedDate = DateTime.Now;

            return loanDisbursementBatch;
        }
    }
}
