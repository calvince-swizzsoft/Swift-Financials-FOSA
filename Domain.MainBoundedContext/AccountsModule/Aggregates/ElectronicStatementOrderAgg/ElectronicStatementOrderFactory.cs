using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg
{
    public static class ElectronicStatementOrderFactory
    {
        public static ElectronicStatementOrder CreateElectronicStatementOrder(Guid customerAccountId, Duration duration, Schedule schedule, string remarks)
        {
            var electronicStatementOrder = new ElectronicStatementOrder();

            electronicStatementOrder.GenerateNewIdentity();

            electronicStatementOrder.CustomerAccountId = customerAccountId;

            electronicStatementOrder.Duration = duration;

            electronicStatementOrder.Schedule = schedule;

            electronicStatementOrder.Remarks = remarks;

            electronicStatementOrder.CreatedDate = DateTime.Now;

            return electronicStatementOrder;
        }
    }
}
