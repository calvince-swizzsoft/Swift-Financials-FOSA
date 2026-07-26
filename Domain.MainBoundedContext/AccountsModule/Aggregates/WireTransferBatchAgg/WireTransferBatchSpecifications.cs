using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchAgg
{
    public static class WireTransferBatchSpecifications
    {
        public static Specification<WireTransferBatch> DefaultSpec()
        {
            Specification<WireTransferBatch> specification = new TrueSpecification<WireTransferBatch>();

            return specification;
        }

        public static Specification<WireTransferBatch> WireTransferBatchesWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<WireTransferBatch> specification = new DirectSpecification<WireTransferBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<WireTransferBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<WireTransferBatch>(c => c.Reference.Contains(text));
                var wireTransferTypeSpec = new DirectSpecification<WireTransferBatch>(c => c.WireTransferType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<WireTransferBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | wireTransferTypeSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<WireTransferBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
