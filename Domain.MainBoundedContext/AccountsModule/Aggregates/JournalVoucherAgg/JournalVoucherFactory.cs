using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherAgg
{
    public static class JournalVoucherFactory
    {
        public static JournalVoucher CreateJournalVoucher(Guid branchId, Guid postingPeriodId, Guid chartOfAccountId, Guid? customerAccountId, int type, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, DateTime? valueDate)
        {
            var journalVoucher = new JournalVoucher();

            journalVoucher.GenerateNewIdentity();

            journalVoucher.BranchId = branchId;

            journalVoucher.PostingPeriodId = postingPeriodId;

            journalVoucher.ChartOfAccountId = chartOfAccountId;

            journalVoucher.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            journalVoucher.Type = (byte)type;

            journalVoucher.TotalValue = totalValue;

            journalVoucher.PrimaryDescription = primaryDescription;

            journalVoucher.SecondaryDescription = secondaryDescription;

            journalVoucher.Reference = reference;

            journalVoucher.ValueDate = valueDate;

            journalVoucher.CreatedDate = DateTime.Now;

            return journalVoucher;
        }
    }
}
