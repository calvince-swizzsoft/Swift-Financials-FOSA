using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchAgg
{
    public static class OverDeductionBatchSpecifications
    {
        public static Specification<OverDeductionBatch> DefaultSpec()
        {
            Specification<OverDeductionBatch> specification = new TrueSpecification<OverDeductionBatch>();

            return specification;
        }

        public static Specification<OverDeductionBatch> OverDeductionBatchWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<OverDeductionBatch> specification = new DirectSpecification<OverDeductionBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<OverDeductionBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<OverDeductionBatch>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<OverDeductionBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<OverDeductionBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
