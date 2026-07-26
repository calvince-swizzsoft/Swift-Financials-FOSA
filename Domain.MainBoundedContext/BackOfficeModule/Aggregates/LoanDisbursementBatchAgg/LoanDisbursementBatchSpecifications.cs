using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg
{
    public static class LoanDisbursementBatchSpecifications
    {
        public static Specification<LoanDisbursementBatch> DefaultSpec()
        {
            Specification<LoanDisbursementBatch> specification = new TrueSpecification<LoanDisbursementBatch>();

            return specification;
        }

        public static Specification<LoanDisbursementBatch> LoanDisbursementBatchesWithDateRangeAndStatus(DateTime startDate, DateTime endDate, int status, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanDisbursementBatch> specification = new DirectSpecification<LoanDisbursementBatch>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate );

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<LoanDisbursementBatch>(c => c.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<LoanDisbursementBatch>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<LoanDisbursementBatch>(c => c.CreatedBy.Contains(text));

                specification &= (branchSpec | referenceSpec | createdBySpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var batchNumberSpec = new DirectSpecification<LoanDisbursementBatch>(x => x.BatchNumber == number);

                    specification |= batchNumberSpec;
                }
            }

            return specification;
        }
    }
}
