using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg
{
    public static class StandingOrderFactory
    {
        public static StandingOrder CreateStandingOrder(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, Duration duration, Schedule schedule, Charge charge, int trigger, decimal loanAmount, decimal paymentPerPeriod, decimal principal, decimal interest, decimal capitalizedInterest, string remarks, bool chargeable)
        {
            var standingOrder = new StandingOrder();

            standingOrder.GenerateNewIdentity();

            standingOrder.BenefactorCustomerAccountId = benefactorCustomerAccountId;

            standingOrder.BeneficiaryCustomerAccountId = beneficiaryCustomerAccountId;

            standingOrder.Duration = duration;

            standingOrder.Schedule = schedule;

            standingOrder.Charge = charge;

            standingOrder.Trigger = (byte)trigger;

            standingOrder.LoanAmount = loanAmount;

            standingOrder.PaymentPerPeriod = paymentPerPeriod;

            standingOrder.Principal = principal;

            standingOrder.Interest = interest;

            standingOrder.CapitalizedInterest = capitalizedInterest;

            standingOrder.Remarks = remarks;

            standingOrder.Chargeable = chargeable;

            standingOrder.CreatedDate = DateTime.Now;

            return standingOrder;
        }
    }
}
