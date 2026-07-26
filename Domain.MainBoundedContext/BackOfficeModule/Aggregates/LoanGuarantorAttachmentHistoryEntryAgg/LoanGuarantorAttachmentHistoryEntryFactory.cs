using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryEntryAgg
{
    public static class LoanGuarantorAttachmentHistoryEntryFactory
    {
        public static LoanGuarantorAttachmentHistoryEntry CreateLoanGuarantorAttachmentHistoryEntry(Guid loanGuarantorAttachmentHistoryId, Guid loanGuarantorId, Guid destinationCustomerAccountId, decimal principalAttached, decimal interestAttached, string reference)
        {
            var loanGuarantorAttachmentHistoryEntry = new LoanGuarantorAttachmentHistoryEntry();

            loanGuarantorAttachmentHistoryEntry.GenerateNewIdentity();

            loanGuarantorAttachmentHistoryEntry.LoanGuarantorAttachmentHistoryId = loanGuarantorAttachmentHistoryId;

            loanGuarantorAttachmentHistoryEntry.LoanGuarantorId = loanGuarantorId;

            loanGuarantorAttachmentHistoryEntry.DestinationCustomerAccountId = destinationCustomerAccountId;

            loanGuarantorAttachmentHistoryEntry.PrincipalAttached = principalAttached;

            loanGuarantorAttachmentHistoryEntry.InterestAttached = interestAttached;

            loanGuarantorAttachmentHistoryEntry.Reference = reference;

            loanGuarantorAttachmentHistoryEntry.CreatedDate = DateTime.Now;

            return loanGuarantorAttachmentHistoryEntry;
        }
    }
}
