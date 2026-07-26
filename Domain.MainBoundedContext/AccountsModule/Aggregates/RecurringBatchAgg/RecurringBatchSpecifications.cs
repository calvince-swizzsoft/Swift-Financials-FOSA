using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchAgg
{
    public static class RecurringBatchSpecifications
    {
        public static Specification<RecurringBatch> DefaultSpec()
        {
            Specification<RecurringBatch> specification = new TrueSpecification<RecurringBatch>();

            return specification;
        }

        public static Specification<RecurringBatch> RecurringBatchesWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<RecurringBatch> specification = new DirectSpecification<RecurringBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<RecurringBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<RecurringBatch>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<RecurringBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<RecurringBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
