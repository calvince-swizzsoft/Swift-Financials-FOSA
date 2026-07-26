using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg
{
    public static class SalaryCardEntrySpecifications
    {
        public static Specification<SalaryCardEntry> DefaultSpec()
        {
            Specification<SalaryCardEntry> specification = new TrueSpecification<SalaryCardEntry>();

            return specification;
        }

        public static ISpecification<SalaryCardEntry> SalaryCardEntryWithSalaryCardId(Guid salaryCardId)
        {
            Specification<SalaryCardEntry> specification = DefaultSpec();

            if (salaryCardId != null && salaryCardId != Guid.Empty)
            {
                specification &= new DirectSpecification<SalaryCardEntry>(x => x.SalaryCardId == salaryCardId);
            }

            return specification;
        }

        public static ISpecification<SalaryCardEntry> SalaryCardEntryWithSalaryGroupEntryId(Guid salaryGroupEntryId)
        {
            Specification<SalaryCardEntry> specification = DefaultSpec();

            if (salaryGroupEntryId != null && salaryGroupEntryId != Guid.Empty)
            {
                specification &= new DirectSpecification<SalaryCardEntry>(x => x.SalaryGroupEntryId == salaryGroupEntryId);
            }

            return specification;
        }
    }
}
