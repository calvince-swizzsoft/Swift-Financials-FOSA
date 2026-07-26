using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.IncomeAdjustmentAgg
{
    public static class IncomeAdjustmentSpecifications
    {
        public static Specification<IncomeAdjustment> DefaultSpec()
        {
            Specification<IncomeAdjustment> specification = new TrueSpecification<IncomeAdjustment>();

            return specification;
        }

        public static Specification<IncomeAdjustment> IncomeAdjustmentFullText(string text)
        {
            Specification<IncomeAdjustment> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<IncomeAdjustment>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<IncomeAdjustment> IncomeAdjustmentDescription(string Description)
        {
            Specification<IncomeAdjustment> specification = new TrueSpecification<IncomeAdjustment>();

            specification &= new DirectSpecification<IncomeAdjustment>(c => c.Description == Description);

            return specification;
        }
    }
}
