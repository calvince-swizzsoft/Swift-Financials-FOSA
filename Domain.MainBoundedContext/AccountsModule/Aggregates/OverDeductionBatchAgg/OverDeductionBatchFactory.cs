using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchAgg
{
    public static class OverDeductionBatchFactory
    {
        public static OverDeductionBatch CreateOverDeductionBatch(Guid branchId, decimal totalValue, string reference)
        {
            var overDeductionBatch = new OverDeductionBatch();

            overDeductionBatch.GenerateNewIdentity();

            overDeductionBatch.BranchId = branchId;

            overDeductionBatch.TotalValue = totalValue;

            overDeductionBatch.Reference = reference;

            overDeductionBatch.CreatedDate = DateTime.Now;

            return overDeductionBatch;
        }
    }
}
