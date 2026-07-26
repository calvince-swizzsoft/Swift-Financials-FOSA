using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalNotificationAgg
{
    public static class WithdrawalNotificationFactory
    {
        public static WithdrawalNotification CreateWithdrawalNotification(Guid customerId, Guid branchId, int category, string remarks)
        {
            var withdrawalNotification = new WithdrawalNotification();

            withdrawalNotification.GenerateNewIdentity();

            withdrawalNotification.CustomerId = customerId;

            withdrawalNotification.BranchId = branchId;

            withdrawalNotification.Category = category;

            withdrawalNotification.Remarks = remarks;

            withdrawalNotification.CreatedDate = DateTime.Now;

            return withdrawalNotification;
        }
    }
}
