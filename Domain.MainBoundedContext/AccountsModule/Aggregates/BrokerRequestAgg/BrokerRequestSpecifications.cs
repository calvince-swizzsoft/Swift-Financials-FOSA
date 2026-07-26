using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BrokerRequestAgg
{
    public static class BrokerRequestSpecifications
    {
        public static Specification<BrokerRequest> DefaultSpec()
        {
            Specification<BrokerRequest> specification = new TrueSpecification<BrokerRequest>();

            return specification;
        }

        public static Specification<BrokerRequest> BrokerRequestWithDateRangeAndFilter(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<BrokerRequest> specification = new DirectSpecification<BrokerRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!string.IsNullOrWhiteSpace(text))
            {
            }

            return specification;
        }

        public static Specification<BrokerRequest> ThirdPartyNotifiableBrokerRequests(string text, int daysCap)
        {
            Specification<BrokerRequest> specification = new DirectSpecification<BrokerRequest>(c => (SqlFunctions.DateDiff("DD", c.CreatedDate, SqlFunctions.GetUtcDate()) <= daysCap) &&
            c.Status == (int)BrokerRequestStatus.Processed && c.IPNEnabled && c.IPNStatus == (int)InstantPaymentNotificationStatus.Pending);

            if (!string.IsNullOrWhiteSpace(text))
            {
            }

            return specification;
        }

        public static ISpecification<BrokerRequest> BrokerRequestWithId(params Guid[] brokerRequestIds)
        {
            Specification<BrokerRequest> specification = new TrueSpecification<BrokerRequest>();

            var brokerRequestIdSpecs = new List<Specification<BrokerRequest>>();

            if (brokerRequestIds != null)
            {
                Array.ForEach(brokerRequestIds, (brokerRequestId) =>
                {
                    var brokerRequestIdSpec = new DirectSpecification<BrokerRequest>(x => x.Id == brokerRequestId);

                    brokerRequestIdSpecs.Add(brokerRequestIdSpec);
                });

                if (brokerRequestIdSpecs.Any())
                {
                    var spec0 = brokerRequestIdSpecs[0];

                    for (int i = 1; i < brokerRequestIdSpecs.Count; i++)
                    {
                        spec0 |= brokerRequestIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }
    }
}