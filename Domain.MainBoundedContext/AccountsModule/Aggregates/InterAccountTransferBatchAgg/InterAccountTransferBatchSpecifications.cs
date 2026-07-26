using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchAgg
{
    public static class InterAccountTransferBatchSpecifications
    {
        public static Specification<InterAccountTransferBatch> DefaultSpec()
        {
            Specification<InterAccountTransferBatch> specification = new TrueSpecification<InterAccountTransferBatch>();

            return specification;
        }

        public static Specification<InterAccountTransferBatch> InterAccountTransferBatchesWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<InterAccountTransferBatch> specification = new DirectSpecification<InterAccountTransferBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var createdBySpec = new DirectSpecification<InterAccountTransferBatch>(c => c.CreatedBy.Contains(text));
                var referenceSpec = new DirectSpecification<InterAccountTransferBatch>(c => c.Reference.Contains(text));

                specification &= (createdBySpec | referenceSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<InterAccountTransferBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }

        public static ISpecification<InterAccountTransferBatch> InterAccountTransferBatchWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<InterAccountTransferBatch> specification = new DirectSpecification<InterAccountTransferBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var createdBySpec = new DirectSpecification<InterAccountTransferBatch>(c => c.CreatedBy.Contains(text));
                var referenceSpec = new DirectSpecification<InterAccountTransferBatch>(c => c.Reference.Contains(text));

                specification &= (createdBySpec | referenceSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<InterAccountTransferBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
