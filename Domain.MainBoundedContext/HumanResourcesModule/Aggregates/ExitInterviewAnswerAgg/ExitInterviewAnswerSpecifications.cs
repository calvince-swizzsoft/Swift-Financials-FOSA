using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewAnswerAgg
{
    public static class ExitInterviewAnswerSpecifications
    {
        public static Specification<ExitInterviewAnswer> DefaultSpec()
        {
            Specification<ExitInterviewAnswer> specification = new TrueSpecification<ExitInterviewAnswer>();

            return specification;
        }

        public static Specification<ExitInterviewAnswer> ExitInterviewAnswerFullText(string text)
        {
            Specification<ExitInterviewAnswer> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<ExitInterviewAnswer>(c => c.Employee.Customer.Reference3.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec);
            }

            return specification;
        }

        public static ISpecification<ExitInterviewAnswer> ExitInterviewAnswerWithEmployeeId(Guid employeeId)
        {
            Specification<ExitInterviewAnswer> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty)
            {
                specification &= new DirectSpecification<ExitInterviewAnswer>(x => x.EmployeeId == employeeId && !x.IsLocked);
            }

            return specification;
        }

    }
}
