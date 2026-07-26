using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanPurposeAgg
{
    public static class LoanPurposeFactory
    {
        public static LoanPurpose CreateLoanPurpose(string description)
        {
            var loanPurpose = new LoanPurpose();

            loanPurpose.GenerateNewIdentity();

            loanPurpose.Description = description;

            loanPurpose.CreatedDate = DateTime.Now;

            return loanPurpose;
        }
    }
}
