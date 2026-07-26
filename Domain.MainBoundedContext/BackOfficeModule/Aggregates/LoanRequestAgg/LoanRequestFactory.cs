using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRequestAgg
{
    public static class LoanRequestFactory
    {
        public static LoanRequest CreateLoanRequest(Guid customerId, Guid loanProductId, Guid loanPurposeId, decimal amountApplied, DateTime receivedDate, string reference)
        {
            var loanRequest = new LoanRequest();

            loanRequest.GenerateNewIdentity();

            loanRequest.CustomerId = customerId;

            loanRequest.LoanProductId = loanProductId;

            loanRequest.LoanPurposeId = loanPurposeId;

            loanRequest.AmountApplied = amountApplied;

            loanRequest.ReceivedDate = receivedDate;

            loanRequest.Reference = reference;

            loanRequest.CreatedDate = DateTime.Now;

            return loanRequest;
        }
    }
}
