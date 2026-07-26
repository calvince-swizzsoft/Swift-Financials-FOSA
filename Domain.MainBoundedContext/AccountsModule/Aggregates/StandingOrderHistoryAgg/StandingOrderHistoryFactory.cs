using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg
{
    public static class StandingOrderHistoryFactory
    {
        public static StandingOrderHistory CreateStandingOrderHistory(Guid standingOrderId, Guid postingPeriodId, Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, Duration duration, Schedule schedule, Charge charge, int month, int trigger, decimal expectedPrincipal, decimal expectedInterest, decimal actualPrincipal, decimal actualInterest, string remarks)
        {
            var standingOrderHistory = new StandingOrderHistory();

            standingOrderHistory.GenerateNewIdentity();

            standingOrderHistory.StandingOrderId = standingOrderId;

            standingOrderHistory.PostingPeriodId = postingPeriodId;

            standingOrderHistory.BenefactorCustomerAccountId = benefactorCustomerAccountId;

            standingOrderHistory.BeneficiaryCustomerAccountId = beneficiaryCustomerAccountId;

            standingOrderHistory.Duration = duration;

            standingOrderHistory.Schedule = schedule;

            standingOrderHistory.Charge = charge;

            standingOrderHistory.Month = (byte)month;

            standingOrderHistory.Trigger = (byte)trigger;

            standingOrderHistory.ExpectedPrincipal = expectedPrincipal;

            standingOrderHistory.ExpectedInterest = expectedInterest;

            standingOrderHistory.ActualPrincipal = actualPrincipal;

            standingOrderHistory.ActualInterest = actualInterest;
            
            standingOrderHistory.Remarks = remarks;

            standingOrderHistory.CreatedDate = DateTime.Now;

            return standingOrderHistory;
        }
    }
}
