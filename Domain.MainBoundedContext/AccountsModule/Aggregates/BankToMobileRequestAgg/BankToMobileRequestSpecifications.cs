using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankToMobileRequestAgg
{
    public static class BankToMobileRequestSpecifications
    {
        public static Specification<BankToMobileRequest> DefaultSpec()
        {
            Specification<BankToMobileRequest> specification = new TrueSpecification<BankToMobileRequest>();

            return specification;
        }

        public static ISpecification<BankToMobileRequest> BankToMobileRequestWithId(params Guid[] bankToMobileRequestIds)
        {
            Specification<BankToMobileRequest> specification = new TrueSpecification<BankToMobileRequest>();

            var bankToMobileRequestIdSpecs = new List<Specification<BankToMobileRequest>>();

            if (bankToMobileRequestIds != null)
            {
                Array.ForEach(bankToMobileRequestIds, (bankToMobileRequestId) =>
                {
                    var bankToMobileRequestIdSpec = new DirectSpecification<BankToMobileRequest>(x => x.Id == bankToMobileRequestId);

                    bankToMobileRequestIdSpecs.Add(bankToMobileRequestIdSpec);
                });

                if (bankToMobileRequestIdSpecs.Any())
                {
                    var spec0 = bankToMobileRequestIdSpecs[0];

                    for (int i = 1; i < bankToMobileRequestIdSpecs.Count; i++)
                    {
                        spec0 |= bankToMobileRequestIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }

        public static Specification<BankToMobileRequest> BankToMobileRequestWithDateRangeAndFilter(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<BankToMobileRequest> specification = new DirectSpecification<BankToMobileRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
            }

            return specification;
        }

        public static Specification<BankToMobileRequest> ThirdPartyNotifiableBankToMobileRequests(string text, int daysCap)
        {
            Specification<BankToMobileRequest> specification = new DirectSpecification<BankToMobileRequest>(c => (SqlFunctions.DateDiff("DD", c.CreatedDate, SqlFunctions.GetUtcDate()) <= daysCap) &&
            c.Status == (int)BankToMobileRequestStatus.Processed && c.IPNEnabled && c.IPNStatus == (int)InstantPaymentNotificationStatus.Pending);

            if (!String.IsNullOrWhiteSpace(text))
            {
            }

            return specification;
        }
    }
}
