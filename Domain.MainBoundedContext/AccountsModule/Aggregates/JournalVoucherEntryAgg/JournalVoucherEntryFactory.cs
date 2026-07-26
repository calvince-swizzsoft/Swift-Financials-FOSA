using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherEntryAgg
{
    public static class JournalVoucherEntryFactory
    {
        public static JournalVoucherEntry CreateJournalVoucherEntry(Guid journalVoucherId, Guid chartOfAccountId, Guid? customerAccountId, decimal amount)
        {
            var journalVoucherEntry = new JournalVoucherEntry();

            journalVoucherEntry.GenerateNewIdentity();

            journalVoucherEntry.JournalVoucherId = journalVoucherId;

            journalVoucherEntry.ChartOfAccountId = chartOfAccountId;

            journalVoucherEntry.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            journalVoucherEntry.Amount = amount;

            journalVoucherEntry.CreatedDate = DateTime.Now;

            return journalVoucherEntry;
        }
    }
}
