using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalTargetAgg
{
    public static class EmployeeAppraisalTargetSpecifications
    {
        public static Specification<EmployeeAppraisalTarget> DefaultSpec()
        {
            Specification<EmployeeAppraisalTarget> specification = new TrueSpecification<EmployeeAppraisalTarget>();

            return specification;
        }

        public static Specification<EmployeeAppraisalTarget> EmployeeAppraisalTargetFullText(string text)
        {
            Specification<EmployeeAppraisalTarget> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<EmployeeAppraisalTarget>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<EmployeeAppraisalTarget> ParentEmployeeAppraisalTargets()
        {
            Specification<EmployeeAppraisalTarget> specification = new TrueSpecification<EmployeeAppraisalTarget>();

            specification &= new DirectSpecification<EmployeeAppraisalTarget>(c => c.ParentId == null);

            return specification;
        }

        public static ISpecification<EmployeeAppraisalTarget> ChildrenEmployeeAppraisalTargets()
        {
            Specification<EmployeeAppraisalTarget> specification = new TrueSpecification<EmployeeAppraisalTarget>();

            specification &= new DirectSpecification<EmployeeAppraisalTarget>(c => c.ParentId != null && c.Type == (int)EmployeeAppraisalTargetType.DetailEntry && !c.IsLocked);

            return specification;
        }

    }
}
