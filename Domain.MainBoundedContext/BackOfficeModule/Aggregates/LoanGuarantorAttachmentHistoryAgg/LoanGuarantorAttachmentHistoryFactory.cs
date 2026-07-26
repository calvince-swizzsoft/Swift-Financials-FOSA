using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryAgg
{
    public static class LoanGuarantorAttachmentHistoryFactory
    {
        public static LoanGuarantorAttachmentHistory CreateLoanGuarantorAttachmentHistory(Guid sourceCustomerAccountId, decimal principalBalance, decimal interestBalance)
        {
            var loanGuarantorAttachmentHistory = new LoanGuarantorAttachmentHistory();

            loanGuarantorAttachmentHistory.GenerateNewIdentity();

            loanGuarantorAttachmentHistory.SourceCustomerAccountId = sourceCustomerAccountId;

            loanGuarantorAttachmentHistory.PrincipalBalance = principalBalance;

            loanGuarantorAttachmentHistory.InterestBalance = interestBalance;

            loanGuarantorAttachmentHistory.CreatedDate = DateTime.Now;

            return loanGuarantorAttachmentHistory;
        }
    }
}
