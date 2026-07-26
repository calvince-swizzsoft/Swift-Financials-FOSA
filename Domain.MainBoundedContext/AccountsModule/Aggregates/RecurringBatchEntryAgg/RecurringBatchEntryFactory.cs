using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchEntryAgg
{
    public static class RecurringBatchEntryFactory
    {
        public static RecurringBatchEntry CreateRecurringBatchEntry(Guid recurringBatchId, Guid? customerAccountId, Guid? secondaryCustomerAccountId, Guid? standingOrderId, Guid? electronicStatementOrderId, Duration electronicStatement, string reference, string remarks)
        {
            var recurringBatchEntry = new RecurringBatchEntry();

            recurringBatchEntry.GenerateNewIdentity();

            recurringBatchEntry.RecurringBatchId = recurringBatchId;

            recurringBatchEntry.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            recurringBatchEntry.SecondaryCustomerAccountId = (secondaryCustomerAccountId != null && secondaryCustomerAccountId != Guid.Empty) ? secondaryCustomerAccountId : null;

            recurringBatchEntry.StandingOrderId = (standingOrderId != null && standingOrderId != Guid.Empty) ? standingOrderId : null;

            recurringBatchEntry.ElectronicStatementOrderId = (electronicStatementOrderId != null && electronicStatementOrderId != Guid.Empty) ? electronicStatementOrderId : null;

            recurringBatchEntry.ElectronicStatement = electronicStatement;

            recurringBatchEntry.Reference = reference;

            recurringBatchEntry.Remarks = remarks;

            recurringBatchEntry.CreatedDate = DateTime.Now;

            return recurringBatchEntry;
        }
    }
}
