using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchEntryAgg
{
    public static class CreditBatchEntryFactory
    {
        public static CreditBatchEntry CreateCreditBatchEntry(Guid creditBatchId, Guid? customerAccountId, Guid? chartOfAccountId, decimal principal, decimal interest, decimal balance, string beneficiary, string reference)
        {
            var creditBatchEntry = new CreditBatchEntry();

            creditBatchEntry.GenerateNewIdentity();

            creditBatchEntry.CreditBatchId = creditBatchId;

            creditBatchEntry.CustomerAccountId = customerAccountId;

            creditBatchEntry.ChartOfAccountId = chartOfAccountId;

            creditBatchEntry.Principal = principal;

            creditBatchEntry.Interest = interest;

            creditBatchEntry.Balance = balance;

            creditBatchEntry.Beneficiary = beneficiary;

            creditBatchEntry.Reference = reference;

            creditBatchEntry.CreatedDate = DateTime.Now;

            return creditBatchEntry;
        }
    }
}
