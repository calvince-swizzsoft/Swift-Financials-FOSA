using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalSettlementAgg
{
    public static class WithdrawalSettlementSpecifications
    {
        public static Specification<WithdrawalSettlement> DefaultSpec()
        {
            Specification<WithdrawalSettlement> specification = new TrueSpecification<WithdrawalSettlement>();

            return specification;
        }

        public static Specification<WithdrawalSettlement> WithdrawalSettlementByWithdrawalNotificationId(Guid withdrawalNotificationId)
        {
            Specification<WithdrawalSettlement> specification = DefaultSpec();

            if (withdrawalNotificationId != null && withdrawalNotificationId != Guid.Empty)
            {
                var withdrawalNotificationIdSpec = new DirectSpecification<WithdrawalSettlement>(c => c.WithdrawalNotificationId == withdrawalNotificationId);

                specification &= withdrawalNotificationIdSpec;
            }

            return specification;
        }
    }
}
