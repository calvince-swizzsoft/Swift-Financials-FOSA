using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupEntryAgg
{
    public static class SalaryGroupEntrySpecifications
    {
        public static Specification<SalaryGroupEntry> DefaultSpec()
        {
            Specification<SalaryGroupEntry> specification = new TrueSpecification<SalaryGroupEntry>();

            return specification;
        }

        public static ISpecification<SalaryGroupEntry> SalaryGroupEntryWithSalaryGroupId(Guid salaryGroupId)
        {
            Specification<SalaryGroupEntry> specification = DefaultSpec();

            if (salaryGroupId != null && salaryGroupId != Guid.Empty)
            {
                specification &= new DirectSpecification<SalaryGroupEntry>(x => x.SalaryGroupId == salaryGroupId);
            }

            return specification;
        }
    }
}
