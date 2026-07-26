using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg
{
    public static class StandingOrderHistorySpecifications
    {
        public static Specification<StandingOrderHistory> DefaultSpec()
        {
            Specification<StandingOrderHistory> specification = new TrueSpecification<StandingOrderHistory>();

            return specification;
        }

        public static Specification<StandingOrderHistory> StandingOrderHistoryWithStandingOrderId(Guid standingOrderId)
        {
            Specification<StandingOrderHistory> specification = DefaultSpec();

            if (standingOrderId != null && standingOrderId != Guid.Empty)
            {
                var standingOrderIdSpec = new DirectSpecification<StandingOrderHistory>(c => c.StandingOrderId == standingOrderId);

                specification &= standingOrderIdSpec;
            }

            return specification;
        }

        public static Specification<StandingOrderHistory> StandingOrderHistory(Guid standingOrderId, Guid postingPeriodId, int month)
        {
            Specification<StandingOrderHistory> specification = new DirectSpecification<StandingOrderHistory>(c => c.StandingOrderId == standingOrderId && c.PostingPeriodId == postingPeriodId && c.Month == month);

            return specification;
        }

        public static Specification<StandingOrderHistory> StandingOrderHistory(Guid standingOrderId, Guid postingPeriodId, DateTime startDate)
        {
            Specification<StandingOrderHistory> specification = new DirectSpecification<StandingOrderHistory>(c => c.StandingOrderId == standingOrderId && c.PostingPeriodId == postingPeriodId && c.CreatedDate >= startDate);

            return specification;
        }
    }
}
