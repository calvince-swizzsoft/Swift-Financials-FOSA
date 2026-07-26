using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg
{
    public static class LoanDisbursementBatchEntryFactory
    {
        public static LoanDisbursementBatchEntry CreateLoanDisbursementBatchEntry(Guid loanDisbursementBatchId, Guid loanCaseId, string reference)
        {
            var loanDisbursementBatchEntry = new LoanDisbursementBatchEntry();

            loanDisbursementBatchEntry.GenerateNewIdentity();

            loanDisbursementBatchEntry.LoanDisbursementBatchId = loanDisbursementBatchId;

            loanDisbursementBatchEntry.LoanCaseId = loanCaseId;

            loanDisbursementBatchEntry.Reference = reference;

            loanDisbursementBatchEntry.CreatedDate = DateTime.Now;

            return loanDisbursementBatchEntry;
        }
    }
}
