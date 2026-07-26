using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CostCenterAgg
{
    public static class CostCenterSpecifications
    {
        public static Specification<CostCenter> DefaultSpec()
        {
            Specification<CostCenter> specification = new TrueSpecification<CostCenter>();

            return specification;
        }

        public static Specification<CostCenter> CostCenterFullText(string text)
        {
            Specification<CostCenter> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<CostCenter>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }


        public static ISpecification<CostCenter> CostCenterWithCostCenter(string Description)
        {
            Specification<CostCenter> specification = new TrueSpecification<CostCenter>();

            specification &= new DirectSpecification<CostCenter>(c => c.Description == Description);

            return specification;
        }
    }
}
