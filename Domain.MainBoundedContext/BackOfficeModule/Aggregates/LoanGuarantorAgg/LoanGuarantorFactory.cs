using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg
{
    public static class LoanGuarantorFactory
    {
        public static LoanGuarantor CreateLoanGuarantor(Guid customerId, Guid? loaneeCustomerId, Guid? loanProductId, Guid? loanCaseId, decimal totalShares, decimal committedShares, decimal amountGuaranteed, decimal amountPledged, double appraisalFactor)
        {
            var loanGuarantor = new LoanGuarantor();

            loanGuarantor.GenerateNewIdentity();

            loanGuarantor.CustomerId = customerId;

            loanGuarantor.LoaneeCustomerId = (loaneeCustomerId != null && loaneeCustomerId != Guid.Empty) ? loaneeCustomerId : null;

            loanGuarantor.LoanProductId = (loanProductId != null && loanProductId != Guid.Empty) ? loanProductId : null;

            loanGuarantor.LoanCaseId = (loanCaseId != null && loanCaseId != Guid.Empty) ? loanCaseId : null;

            loanGuarantor.TotalShares = totalShares;

            loanGuarantor.CommittedShares = committedShares;

            loanGuarantor.AmountGuaranteed = amountGuaranteed;

            loanGuarantor.AmountPledged = amountPledged;

            loanGuarantor.AppraisalFactor = appraisalFactor;

            loanGuarantor.CreatedDate = DateTime.Now;

            return loanGuarantor;
        }
    }
}
