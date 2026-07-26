using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonCommissionAgg
{
    public static class UnPayReasonCommissionFactory
    {
        public static UnPayReasonCommission CreateUnPayReasonCommission(Guid unPayReasonId, Guid commissionId)
        {
            var unPayReasonCommission = new UnPayReasonCommission();

            unPayReasonCommission.GenerateNewIdentity();

            unPayReasonCommission.UnPayReasonId = unPayReasonId;

            unPayReasonCommission.CommissionId = commissionId;

            unPayReasonCommission.CreatedDate = DateTime.Now;

            return unPayReasonCommission;
        }
    }
}
