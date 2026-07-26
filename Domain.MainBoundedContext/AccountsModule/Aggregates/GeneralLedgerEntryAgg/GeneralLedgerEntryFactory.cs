using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerEntryAgg
{
    public static class GeneralLedgerEntryFactory
    {
        public static GeneralLedgerEntry CreateGeneralLedgerEntry(Guid generalLedgerId, Guid branchId, Guid chartOfAccountId, Guid contraChartOfAccountId, Guid? customerAccountId, Guid? contraCustomerAccountId, string primaryDescription, string secondaryDescription, string reference, decimal amount, DateTime? valueDate)
        {
            var generalLedgerEntry = new GeneralLedgerEntry();

            generalLedgerEntry.GenerateNewIdentity();

            generalLedgerEntry.GeneralLedgerId = generalLedgerId;

            generalLedgerEntry.BranchId = branchId;

            generalLedgerEntry.ChartOfAccountId = chartOfAccountId;

            generalLedgerEntry.ContraChartOfAccountId = contraChartOfAccountId;

            generalLedgerEntry.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            generalLedgerEntry.ContraCustomerAccountId = (contraCustomerAccountId != null && contraCustomerAccountId != Guid.Empty) ? contraCustomerAccountId : null;

            generalLedgerEntry.PrimaryDescription = primaryDescription;

            generalLedgerEntry.SecondaryDescription = secondaryDescription;

            generalLedgerEntry.Reference = reference;

            generalLedgerEntry.Amount = amount;

            generalLedgerEntry.ValueDate = valueDate;

            generalLedgerEntry.CreatedDate = DateTime.Now;

            return generalLedgerEntry;
        }
    }
}
