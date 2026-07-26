using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalSettlementAgg
{
    public static class WithdrawalSettlementFactory
    {
        public static WithdrawalSettlement CreateWithdrawalSettlement(Guid withdrawalNotificationId, Guid customerAccountId, decimal principal, decimal interest, decimal carryForwards, string reference)
        {
            var withdrawalSettlement = new WithdrawalSettlement();

            withdrawalSettlement.GenerateNewIdentity();

            withdrawalSettlement.WithdrawalNotificationId = withdrawalNotificationId;

            withdrawalSettlement.CustomerAccountId = customerAccountId;

            withdrawalSettlement.Principal = principal;

            withdrawalSettlement.Interest = interest;

            withdrawalSettlement.CarryForwards = carryForwards;

            withdrawalSettlement.Reference = reference;

            withdrawalSettlement.CreatedDate = DateTime.Now;

            return withdrawalSettlement;
        }
    }
}
