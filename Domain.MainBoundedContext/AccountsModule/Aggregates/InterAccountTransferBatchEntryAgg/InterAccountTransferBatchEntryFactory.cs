using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchEntryAgg
{
    public static class InterAccountTransferBatchEntryFactory
    {
        public static InterAccountTransferBatchEntry CreateInterAccountTransferBatchEntry(Guid interAccountTransferBatchId, int apportionTo, Guid? customerAccountId, Guid? chartOfAccountId, decimal principal, decimal interest, string primaryDescription, string secondaryDescription, string reference)
        {
            var interAccountTransferBatchEntry = new InterAccountTransferBatchEntry();

            interAccountTransferBatchEntry.GenerateNewIdentity();

            interAccountTransferBatchEntry.InterAccountTransferBatchId = interAccountTransferBatchId;

            interAccountTransferBatchEntry.ApportionTo = (byte)apportionTo;

            interAccountTransferBatchEntry.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            interAccountTransferBatchEntry.ChartOfAccountId = (chartOfAccountId != null && chartOfAccountId != Guid.Empty) ? chartOfAccountId : null;

            interAccountTransferBatchEntry.Principal = principal;

            interAccountTransferBatchEntry.Interest = interest;

            interAccountTransferBatchEntry.PrimaryDescription = primaryDescription;

            interAccountTransferBatchEntry.SecondaryDescription = secondaryDescription;

            interAccountTransferBatchEntry.Reference = reference;

            interAccountTransferBatchEntry.CreatedDate = DateTime.Now;

            return interAccountTransferBatchEntry;
        }
    }
}
