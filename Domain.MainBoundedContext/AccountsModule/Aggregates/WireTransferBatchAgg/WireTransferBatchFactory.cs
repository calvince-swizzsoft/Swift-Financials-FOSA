using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchAgg
{
    public static class WireTransferBatchFactory
    {
        public static WireTransferBatch CreateWireTransferBatch(Guid wireTransferTypeId, Guid branchId, decimal totalValue, int type, string reference, int priority)
        {
            var wireTransferBatch = new WireTransferBatch();

            wireTransferBatch.GenerateNewIdentity();

            wireTransferBatch.WireTransferTypeId = wireTransferTypeId;

            wireTransferBatch.BranchId = branchId;

            wireTransferBatch.TotalValue = totalValue;

            wireTransferBatch.Type = (byte)type;

            wireTransferBatch.Reference = reference;

            wireTransferBatch.Priority = (byte)priority;

            wireTransferBatch.CreatedDate = DateTime.Now;

            return wireTransferBatch;
        }
    }
}
