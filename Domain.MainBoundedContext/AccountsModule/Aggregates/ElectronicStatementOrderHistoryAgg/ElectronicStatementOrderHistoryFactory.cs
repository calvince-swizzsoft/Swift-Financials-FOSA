using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderHistoryAgg
{
    public static class ElectronicStatementOrderHistoryFactory
    {
        public static ElectronicStatementOrderHistory CreateElectronicStatementOrderHistory(Guid electronicStatementOrderId, Guid customerAccountId, Duration duration, Schedule schedule, string sender, string remarks)
        {
            var electronicStatementOrderHistory = new ElectronicStatementOrderHistory();

            electronicStatementOrderHistory.GenerateNewIdentity();

            electronicStatementOrderHistory.ElectronicStatementOrderId = electronicStatementOrderId;

            electronicStatementOrderHistory.CustomerAccountId = customerAccountId;

            electronicStatementOrderHistory.Duration = duration;

            electronicStatementOrderHistory.Schedule = schedule;

            electronicStatementOrderHistory.Sender = sender;

            electronicStatementOrderHistory.Remarks = remarks;

            electronicStatementOrderHistory.CreatedDate = DateTime.Now;

            return electronicStatementOrderHistory;
        }
    }
}
