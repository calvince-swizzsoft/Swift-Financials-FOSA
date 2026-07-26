using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchAgg
{
    public static class JournalReversalBatchSpecifications
    {
        public static Specification<JournalReversalBatch> DefaultSpec()
        {
            Specification<JournalReversalBatch> specification = new TrueSpecification<JournalReversalBatch>();

            return specification;
        }

        public static Specification<JournalReversalBatch> JournalReversalBatchWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<JournalReversalBatch> specification = new DirectSpecification<JournalReversalBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<JournalReversalBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<JournalReversalBatch>(c => c.Remarks.Contains(text));
                var createdBySpec = new DirectSpecification<JournalReversalBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<JournalReversalBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
