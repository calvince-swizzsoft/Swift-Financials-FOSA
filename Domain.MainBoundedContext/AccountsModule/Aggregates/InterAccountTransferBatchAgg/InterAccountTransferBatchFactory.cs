using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchAgg
{
    public static class InterAccountTransferBatchFactory
    {
        public static InterAccountTransferBatch CreateInterAccountTransferBatch(Guid branchId, Guid customerAccountId, string reference)
        {
            var interAccountTransferBatch = new InterAccountTransferBatch();

            interAccountTransferBatch.GenerateNewIdentity();

            interAccountTransferBatch.BranchId = branchId;

            interAccountTransferBatch.CustomerAccountId = customerAccountId;

            interAccountTransferBatch.Reference = reference;

            interAccountTransferBatch.CreatedDate = DateTime.Now;

            return interAccountTransferBatch;
        }
    }
}
