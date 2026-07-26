using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeCommissionAgg
{
    public static class DynamicChargeCommissionFactory
    {
        public static DynamicChargeCommission CreateDynamicChargeCommission(Guid dynamicChargeId, Guid commissionId)
        {
            var dynamicChargeInCommission = new DynamicChargeCommission();

            dynamicChargeInCommission.GenerateNewIdentity();

            dynamicChargeInCommission.DynamicChargeId = dynamicChargeId;

            dynamicChargeInCommission.CommissionId = commissionId;

            dynamicChargeInCommission.CreatedDate = DateTime.Now;

            return dynamicChargeInCommission;
        }
    }
}
