using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanCycleAgg
{
    public static class LoanCycleFactory
    {
        public static LoanCycle CreateLoanCycle(Guid loanProductId, Range range)
        {
            var loanCycle = new LoanCycle();

            loanCycle.GenerateNewIdentity();

            loanCycle.LoanProductId = loanProductId;

            loanCycle.Range = range;

            loanCycle.CreatedDate = DateTime.Now;

            return loanCycle;
        }
    }
}
