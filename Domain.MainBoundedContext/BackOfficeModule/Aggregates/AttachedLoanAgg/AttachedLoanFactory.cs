using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.AttachedLoanAgg
{
    public static class AttachedLoanFactory
    {
        public static AttachedLoan CreateAttachedLoan(Guid loanCaseId, Guid customerAccountId, decimal principalBalance, decimal interestBalance, decimal carryForwardsBalance, decimal clearanceCharges)
        {
            var attachedLoan = new AttachedLoan();

            attachedLoan.GenerateNewIdentity();

            attachedLoan.LoanCaseId = loanCaseId;

            attachedLoan.CustomerAccountId = customerAccountId;

            attachedLoan.PrincipalBalance = principalBalance;

            attachedLoan.InterestBalance = interestBalance;

            attachedLoan.CarryForwardsBalance = carryForwardsBalance;

            attachedLoan.ClearanceCharges = clearanceCharges;

            attachedLoan.CreatedDate = DateTime.Now;

            return attachedLoan;
        }
    }
}
