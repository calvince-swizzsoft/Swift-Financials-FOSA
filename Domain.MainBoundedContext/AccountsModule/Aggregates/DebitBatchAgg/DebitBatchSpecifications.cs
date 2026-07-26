using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchAgg
{
    public static class DebitBatchSpecifications
    {
        public static Specification<DebitBatch> DefaultSpec()
        {
            Specification<DebitBatch> specification = new TrueSpecification<DebitBatch>();

            return specification;
        }

        public static Specification<DebitBatch> DebitBatchFullText(string text)
        {
            Specification<DebitBatch> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<DebitBatch>(c => c.Reference.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }



        public static Specification<DebitBatch> DebitBatchesWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<DebitBatch> specification = new DirectSpecification<DebitBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<DebitBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<DebitBatch>(c => c.Reference.Contains(text));
                var debitTypeSpec = new DirectSpecification<DebitBatch>(c => c.DebitType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<DebitBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | debitTypeSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<DebitBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
