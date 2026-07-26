using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg
{
    public static class CreditBatchSpecifications
    {
        public static Specification<CreditBatch> DefaultSpec()
        {
            Specification<CreditBatch> specification = new TrueSpecification<CreditBatch>();

            return specification;
        }

        public static Specification<CreditBatch> CreditBatchFullText(string text)
        {
            Specification<CreditBatch> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<CreditBatch>(c => c.Reference.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static Specification<CreditBatch> CreditBatchesWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CreditBatch> specification = new DirectSpecification<CreditBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<CreditBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<CreditBatch>(c => c.Reference.Contains(text));
                var creditTypeSpec = new DirectSpecification<CreditBatch>(c => c.CreditType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<CreditBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | creditTypeSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<CreditBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }



        public static Specification<CreditBatch> CreditBatchesWithDateRangeAndStatus(DateTime startDate, DateTime endDate, int status, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CreditBatch> specification = new DirectSpecification<CreditBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<CreditBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<CreditBatch>(c => c.Reference.Contains(text));
                var creditTypeSpec = new DirectSpecification<CreditBatch>(c => c.CreditType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<CreditBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | creditTypeSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<CreditBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
