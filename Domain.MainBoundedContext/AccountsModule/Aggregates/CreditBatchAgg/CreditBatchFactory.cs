using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg
{
    public static class CreditBatchFactory
    {
        public static CreditBatch CreateCreditBatch(Guid creditTypeId, Guid branchId, Guid? postingPeriodId, decimal totalValue, int type, string reference, int month, Charge concession, int priority)
        {
            var creditBatch = new CreditBatch();

            creditBatch.GenerateNewIdentity();

            creditBatch.CreditTypeId = creditTypeId;

            creditBatch.BranchId = branchId;

            creditBatch.PostingPeriodId = (postingPeriodId != null && postingPeriodId != Guid.Empty) ? postingPeriodId : null; 

            creditBatch.TotalValue = totalValue;

            creditBatch.Type = type;

            creditBatch.Reference = reference;

            creditBatch.Month = (byte)month;

            creditBatch.Concession = concession;

            creditBatch.Priority = (byte)priority;

            creditBatch.CreatedDate = DateTime.Now;

            return creditBatch;
        }
    }
}
