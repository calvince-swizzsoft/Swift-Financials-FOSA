using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DepartmentAgg
{
    public static class DepartmentSpecifications
    {
        public static Specification<Department> DefaultSpec()
        {
            Specification<Department> specification = new TrueSpecification<Department>();

            return specification;
        }

        public static Specification<Department> RegistryDepartment()
        {
            return new DirectSpecification<Department>(x => !x.IsLocked && x.IsRegistry);
        }

        public static Specification<Department> DepartmentFullText(string text)
        {
            Specification<Department> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Department>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
