using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoaningRemarkAgg
{
    public static class LoaningRemarkFactory
    {
        public static LoaningRemark CreateLoaningRemark(string description)
        {
            var loaningRemark = new LoaningRemark();

            loaningRemark.GenerateNewIdentity();

            loaningRemark.Description = description;

            loaningRemark.CreatedDate = DateTime.Now;

            return loaningRemark;
        }
    }
}
