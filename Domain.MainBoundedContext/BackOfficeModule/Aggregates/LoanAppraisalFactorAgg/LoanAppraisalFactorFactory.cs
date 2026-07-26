using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanAppraisalFactorAgg
{
    public static class LoanAppraisalFactorFactory
    {
        public static LoanAppraisalFactor CreateLoanAppraisalFactor(Guid loanCaseId, string description, int type, decimal amount) 
        {
            var loanAppraisalFactor = new LoanAppraisalFactor();

            loanAppraisalFactor.GenerateNewIdentity();

            loanAppraisalFactor.LoanCaseId = loanCaseId;

            loanAppraisalFactor.Description = description;

            loanAppraisalFactor.Type = type;

            loanAppraisalFactor.Amount = amount;

            loanAppraisalFactor.CreatedDate = DateTime.Now;

            return loanAppraisalFactor;
        }
    }
}
