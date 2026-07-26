using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeExitAgg
{
    public static class EmployeeExitSpecifications
    {
        public static Specification<EmployeeExit> DefaultSpec()
        {
            Specification<EmployeeExit> specification = new TrueSpecification<EmployeeExit>();

            return specification;
        }

        public static Specification<EmployeeExit> EmployeeExitFullText(string text)
        {
            Specification<EmployeeExit> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<EmployeeExit>(c => c.Employee.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<EmployeeExit>(c => c.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<EmployeeExit>(c => c.Employee.Customer.Reference3.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec);
            }

            return specification;
        }

        public static Specification<EmployeeExit> EmployeeExitsByStatusWithDateRange(int status, string text, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<EmployeeExit> specification = new DirectSpecification<EmployeeExit>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fulltextSpec = EmployeeExitFullText(text);

                specification &= (fulltextSpec);
            }

            return specification;
        }

        public static ISpecification<EmployeeExit> EmployeeExitByEmployeeId(Guid employeeId)
        {
            Specification<EmployeeExit> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty)
            {
                specification &= new DirectSpecification<EmployeeExit>(x => x.EmployeeId == employeeId);
            }

            return specification;
        }

    }
}
