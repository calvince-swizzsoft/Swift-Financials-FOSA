using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchAgg
{
    public static class DebitBatchFactory
    {
        public static DebitBatch CreateDebitBatch(Guid debitTypeId, Guid branchId, string reference, int priority)
        {
            var debitBatch = new DebitBatch();

            debitBatch.GenerateNewIdentity();

            debitBatch.DebitTypeId = debitTypeId;

            debitBatch.BranchId = branchId;

            debitBatch.Reference = reference;

            debitBatch.Priority = (byte)priority;

            debitBatch.CreatedDate = DateTime.Now;

            return debitBatch;
        }
    }
}
