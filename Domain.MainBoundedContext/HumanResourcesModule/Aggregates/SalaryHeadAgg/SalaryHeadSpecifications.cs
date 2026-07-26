using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryHeadAgg
{
    public static class SalaryHeadSpecifications
    {
        public static Specification<SalaryHead> DefaultSpec()
        {
            Specification<SalaryHead> specification = new TrueSpecification<SalaryHead>();

            return specification;
        }

        public static Specification<SalaryHead> SalaryHeadWithType(int salaryHeadType)
        {
            Specification<SalaryHead> specification = new DirectSpecification<SalaryHead>(x => x.Type == salaryHeadType);

            return specification;
        }

        public static Specification<SalaryHead> SalaryHeadFullText(string text)
        {
            Specification<SalaryHead> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<SalaryHead>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
