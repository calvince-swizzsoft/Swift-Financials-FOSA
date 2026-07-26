using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg
{
    public static class CommissionSpecifications
    {
        public static Specification<Commission> DefaultSpec()
        {
            Specification<Commission> specification = new TrueSpecification<Commission>();

            return specification;
        }

        public static Specification<Commission> CommissionFullText(string text)
        {
            Specification<Commission> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Commission>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }


        public static ISpecification<Commission> CommissionWithCommission(string Description)
        {
            Specification<Commission> specification = new TrueSpecification<Commission>();

            specification &= new DirectSpecification<Commission>(c => c.Description == Description);

            return specification;
        }
    }
}
