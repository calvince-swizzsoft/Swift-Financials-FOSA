using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SystemTransactionTypeInCommissionAgg
{
    public static class SystemTransactionTypeInCommissionFactory
    {
        public static SystemTransactionTypeInCommission CreateSystemTransactionTypeInCommission(int systemTransactionType, Guid commissionId, Charge complement)
        {
            var systemTransactionTypeInCommission = new SystemTransactionTypeInCommission();

            systemTransactionTypeInCommission.GenerateNewIdentity();

            systemTransactionTypeInCommission.SystemTransactionType = systemTransactionType;

            systemTransactionTypeInCommission.CommissionId = commissionId;

            systemTransactionTypeInCommission.Complement = complement;

            systemTransactionTypeInCommission.CreatedDate = DateTime.Now;

            return systemTransactionTypeInCommission;
        }
    }
}
