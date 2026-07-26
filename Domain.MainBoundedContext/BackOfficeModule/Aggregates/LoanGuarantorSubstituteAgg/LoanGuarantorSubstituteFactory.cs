using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorSubstituteAgg
{
    public static class LoanGuarantorSubstituteFactory
    {
        public static LoanGuarantorSubstitute CreateLoanGuarantorSubstitute(Guid loanGuarantorId, Guid previousCustomerId, Guid currentCustomerId)
        {
            var loanGuarantorSubstitute = new LoanGuarantorSubstitute();

            loanGuarantorSubstitute.GenerateNewIdentity();

            loanGuarantorSubstitute.LoanGuarantorId = loanGuarantorId;

            loanGuarantorSubstitute.PreviousCustomerId = previousCustomerId;

            loanGuarantorSubstitute.CurrentCustomerId = currentCustomerId;

            loanGuarantorSubstitute.CreatedDate = DateTime.Now;

            return loanGuarantorSubstitute;
        }
    }
}
