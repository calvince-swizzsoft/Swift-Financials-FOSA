using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.SuperSaverPayableAgg
{
    public static class SuperSaverPayableSpecifications
    {
        public static Specification<SuperSaverPayable> DefaultSpec()
        {
            Specification<SuperSaverPayable> specification = new TrueSpecification<SuperSaverPayable>();

            return specification;
        }

        public static Specification<SuperSaverPayable> SuperSaverPayableByStatusWithDateRange(int status, string text, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<SuperSaverPayable> specification = new DirectSpecification<SuperSaverPayable>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var createdBySpec = new DirectSpecification<SuperSaverPayable>(c => c.CreatedBy.Contains(text));

                specification &= (createdBySpec);
            }

            return specification;
        }
    }
}
