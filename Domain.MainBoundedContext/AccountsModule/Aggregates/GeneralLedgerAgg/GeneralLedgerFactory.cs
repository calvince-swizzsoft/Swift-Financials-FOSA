using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerAgg
{
    public static class GeneralLedgerFactory
    {
        public static GeneralLedger CreateGeneralLedger(Guid branchId, Guid postingPeriodId, decimal totalValue, string remarks)
        {
            var generalLedger = new GeneralLedger();

            generalLedger.GenerateNewIdentity();

            generalLedger.BranchId = branchId;

            generalLedger.PostingPeriodId = postingPeriodId;

            generalLedger.TotalValue = totalValue;

            generalLedger.Remarks = remarks;

            generalLedger.CreatedDate = DateTime.Now;

            return generalLedger;
        }
    }
}
