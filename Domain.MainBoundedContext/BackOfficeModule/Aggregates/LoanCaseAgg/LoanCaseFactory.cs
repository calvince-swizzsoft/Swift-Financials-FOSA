using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg
{
    public static class LoanCaseFactory
    {
        public static LoanCase CreateLoanCase(Guid? parentLoanCaseId, Guid branchId, Guid customerId, Guid loanProductId, Guid? loanPurposeId, Guid? savingsProductId, string remarks, decimal amountApplied, DateTime receivedDate, decimal loanProductInvestmentsBalance, decimal loanProductLoanBalance, decimal totalLoansBalance, decimal loanProductLatestIncome, string reference, LoanInterest loanInterest, LoanRegistration loanRegistration, double maximumAmountPercentage, Charge takeHome)
        {
            var loanCase = new LoanCase();

            loanCase.GenerateNewIdentity();

            loanCase.ParentId = (parentLoanCaseId != null && parentLoanCaseId != Guid.Empty) ? parentLoanCaseId : null;

            loanCase.BranchId = branchId;

            loanCase.CustomerId = customerId;

            loanCase.LoanProductId = loanProductId;

            loanCase.LoanPurposeId = (loanPurposeId != null && loanPurposeId != Guid.Empty) ? loanPurposeId : null;

            loanCase.SavingsProductId = (savingsProductId != null && savingsProductId != Guid.Empty) ? savingsProductId : null;

            loanCase.Remarks = remarks;

            loanCase.AmountApplied = amountApplied;

            loanCase.ReceivedDate = receivedDate;

            loanCase.LoanProductInvestmentsBalance = loanProductInvestmentsBalance;

            loanCase.LoanProductLoanBalance = loanProductLoanBalance;

            loanCase.TotalLoansBalance = totalLoansBalance;

            loanCase.LoanProductLatestIncome = loanProductLatestIncome;

            loanCase.LoanInterest = loanInterest;

            loanCase.LoanRegistration = loanRegistration;

            loanCase.MaximumAmountPercentage = maximumAmountPercentage;

            loanCase.TakeHome = takeHome;

            loanCase.Reference = reference;

            loanCase.CreatedDate = DateTime.Now;

            return loanCase;
        }
    }
}
