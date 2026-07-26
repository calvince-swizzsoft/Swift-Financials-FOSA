using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchEntryAgg
{
    public static class OverDeductionBatchEntryFactory
    {
        public static OverDeductionBatchEntry CreateOverDeductionBatchEntry(Guid overDeductionBatchId, Guid debitCustomerAccountId, Guid creditCustomerAccountId, decimal principal, decimal interest)
        {
            var overDeductionBatchEntry = new OverDeductionBatchEntry();

            overDeductionBatchEntry.GenerateNewIdentity();

            overDeductionBatchEntry.OverDeductionBatchId = overDeductionBatchId;

            overDeductionBatchEntry.DebitCustomerAccountId = debitCustomerAccountId;

            overDeductionBatchEntry.CreditCustomerAccountId = creditCustomerAccountId;

            overDeductionBatchEntry.Principal = principal;

            overDeductionBatchEntry.Interest = interest;

            overDeductionBatchEntry.CreatedDate = DateTime.Now;

            return overDeductionBatchEntry;
        }
    }
}
