using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDisciplinaryCaseAgg
{
    public class EmployeeDisciplinaryCaseSpecifications
    {
        public static Specification<EmployeeDisciplinaryCase> DefaultSpec()
        {
            Specification<EmployeeDisciplinaryCase> specification = new TrueSpecification<EmployeeDisciplinaryCase>();

            return specification;
        }

        public static Specification<EmployeeDisciplinaryCase> EmployeeDisciplinaryCaseWithText(string text)
        {
            Specification<EmployeeDisciplinaryCase> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var nameSpec = new DirectSpecification<EmployeeDisciplinaryCase>(c => c.Employee.Customer.Individual.FirstName.Equals(text) || c.Employee.Customer.Individual.LastName.Equals(text));

                var titleSpec = new DirectSpecification<EmployeeDisciplinaryCase>(c => c.FileTitle.Contains(text));

                var descriptionSpec = new DirectSpecification<EmployeeDisciplinaryCase>(c => c.FileDescription.Contains(text));

                specification &= titleSpec || descriptionSpec;
            }

            return specification;
        }

        public static ISpecification<EmployeeDisciplinaryCase> EmployeeDisciplinaryCaseWithEmployeeId(Guid employeeId)
        {
            Specification<EmployeeDisciplinaryCase> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty)
            {
                specification &= new DirectSpecification<EmployeeDisciplinaryCase>(x => x.EmployeeId == employeeId);
            }

            return specification;
        }
    }
}