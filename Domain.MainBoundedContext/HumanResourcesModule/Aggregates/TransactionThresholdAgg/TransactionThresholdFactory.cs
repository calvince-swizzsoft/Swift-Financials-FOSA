using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TransactionThresholdAgg
{
    public static class TransactionThresholdFactory
    {
        public static TransactionThreshold CreateTransactionThreshold(Guid designationId, int type, decimal threshold)
        {
            var transactionThreshold = new TransactionThreshold();

            transactionThreshold.GenerateNewIdentity();

            transactionThreshold.DesignationId = designationId;

            transactionThreshold.Type = (short)type;

            transactionThreshold.Threshold = threshold;

            transactionThreshold.CreatedDate = DateTime.Now;

            return transactionThreshold;
        }
    }
}
