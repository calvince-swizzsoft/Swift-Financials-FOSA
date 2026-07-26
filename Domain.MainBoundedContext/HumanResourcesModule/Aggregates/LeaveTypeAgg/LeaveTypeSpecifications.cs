using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveTypeAgg
{
    public class LeaveTypeSpecifications
    {
        public static Specification<LeaveType> DefaultSpec()
        {
            Specification<LeaveType> specification = new TrueSpecification<LeaveType>();

            return specification;
        }

        public static Specification<LeaveType> LeaveTypeFullText(string filterText)
        {
            Specification<LeaveType> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(filterText))
            {
                var descriptionSpec = new DirectSpecification<LeaveType>(c => c.Description.Contains(filterText));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
