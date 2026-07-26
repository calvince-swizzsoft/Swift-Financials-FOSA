using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalAgg
{
    public static class EmployeeAppraisalSpecifications
    {
        public static Specification<EmployeeAppraisal> DefaultSpec()
        {
            Specification<EmployeeAppraisal> specification = new TrueSpecification<EmployeeAppraisal>();

            return specification;
        }

        public static Specification<EmployeeAppraisal> EmployeeAppraisalFullText(string text)
        {
            Specification<EmployeeAppraisal> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var employeeFirstNameSpec = new DirectSpecification<EmployeeAppraisal>(c => c.Employee.Customer.Individual.FirstName.Contains(text));

                var employeeLastNameSpec = new DirectSpecification<EmployeeAppraisal>(c => c.Employee.Customer.Individual.LastName.Contains(text));

                var employeeIdentityNumberSpec = new DirectSpecification<EmployeeAppraisal>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));

                specification &= (employeeFirstNameSpec | employeeLastNameSpec | employeeIdentityNumberSpec);
            }

            return specification;
        }

        public static ISpecification<EmployeeAppraisal> EmployeeAppraisalWithEmployeeIdAndEmployeeAppraisalPeriodId(Guid employeeId, Guid employeeAppraisalPeriodId)
        {
            Specification<EmployeeAppraisal> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty && employeeAppraisalPeriodId != null && employeeAppraisalPeriodId != Guid.Empty)
            {
                specification &= new DirectSpecification<EmployeeAppraisal>(x => x.EmployeeId == employeeId && x.EmployeeAppraisalPeriodId == employeeAppraisalPeriodId);
            }

            return specification;
        }
    }
}
