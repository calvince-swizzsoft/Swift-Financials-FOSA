using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerAgg
{
    public static class GeneralLedgerSpecifications
    {
        public static Specification<GeneralLedger> DefaultSpec()
        {
            Specification<GeneralLedger> specification = new TrueSpecification<GeneralLedger>();

            return specification;
        }

        public static Specification<GeneralLedger> GeneralLedgersWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<GeneralLedger> specification = new DirectSpecification<GeneralLedger>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<GeneralLedger>(c => c.Remarks.Contains(text));
                var createdBySpec = new DirectSpecification<GeneralLedger>(c => c.CreatedBy.Contains(text));

                specification &= (createdBySpec | remarksSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var ledgerNumberSpec = new DirectSpecification<GeneralLedger>(x => x.LedgerNumber == number);

                    specification |= ledgerNumberSpec;
                }
            }

            return specification;
        }

        public static ISpecification<GeneralLedger> GeneralLedgerWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<GeneralLedger> specification = new DirectSpecification<GeneralLedger>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<GeneralLedger>(c => c.Remarks.Contains(text));
                var createdBySpec = new DirectSpecification<GeneralLedger>(c => c.CreatedBy.Contains(text));

                specification &= ( createdBySpec | remarksSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var ledgerNumberSpec = new DirectSpecification<GeneralLedger>(x => x.LedgerNumber == number);

                    specification |= ledgerNumberSpec;
                }
            }

            return specification;
        }
    }
}
