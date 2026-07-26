using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupAgg
{
    public static class SalaryGroupSpecifications
    {
        public static Specification<SalaryGroup> DefaultSpec()
        {
            Specification<SalaryGroup> specification = new TrueSpecification<SalaryGroup>();

            return specification;
        }

        public static Specification<SalaryGroup> SalaryGroupFullText(string text)
        {
            Specification<SalaryGroup> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<SalaryGroup>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
