using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerEntryAgg
{
    public static class GeneralLedgerEntrySpecifications
    {
        public static Specification<GeneralLedgerEntry> DefaultSpec()
        {
            Specification<GeneralLedgerEntry> specification = new TrueSpecification<GeneralLedgerEntry>();

            return specification;
        }

        public static Specification<GeneralLedgerEntry> GeneralLedgerEntryWithGeneralLedgerId(Guid generalLedgerId)
        {
            Specification<GeneralLedgerEntry> specification = DefaultSpec();

            if (generalLedgerId != null && generalLedgerId != Guid.Empty)
            {
                var generalLedgerIdSpec = new DirectSpecification<GeneralLedgerEntry>(c => c.GeneralLedgerId == generalLedgerId);

                specification &= generalLedgerIdSpec;
            }

            return specification;
        }

        public static Specification<GeneralLedgerEntry> PostedGeneralLedgerEntryWithGeneralLedgerId(Guid generalLedgerId)
        {
            Specification<GeneralLedgerEntry> specification = new DirectSpecification<GeneralLedgerEntry>(x => x.GeneralLedgerId == generalLedgerId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }
    }
}
