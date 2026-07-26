using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationPeriodAgg
{
    public static class AlternateChannelReconciliationPeriodSpecifications
    {
        public static Specification<AlternateChannelReconciliationPeriod> DefaultSpec()
        {
            Specification<AlternateChannelReconciliationPeriod> specification = new TrueSpecification<AlternateChannelReconciliationPeriod>();

            return specification;
        }

        public static Specification<AlternateChannelReconciliationPeriod> AlternateChannelReconciliationPeriodFullText(string text)
        {
            Specification<AlternateChannelReconciliationPeriod> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<AlternateChannelReconciliationPeriod>(c => c.Remarks.Contains(text));

                specification &= (remarksSpec);
            }

            return specification;
        }

        public static Specification<AlternateChannelReconciliationPeriod> AlternateChannelReconciliationPeriodFullText(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<AlternateChannelReconciliationPeriod> specification = new DirectSpecification<AlternateChannelReconciliationPeriod>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<AlternateChannelReconciliationPeriod>(c => c.Remarks.Contains(text));

                var createdBySpec = new DirectSpecification<AlternateChannelReconciliationPeriod>(c => c.CreatedBy.Contains(text));

                specification &= (remarksSpec | createdBySpec);
            }

            return specification;
        }
    }
}
