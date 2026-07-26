using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg
{
    public static class SalaryCardSpecifications
    {
        public static Specification<SalaryCard> DefaultSpec()
        {
            Specification<SalaryCard> specification = new TrueSpecification<SalaryCard>();

            return specification;
        }

        public static ISpecification<SalaryCard> SalaryCardWithEmployeeId(Guid employeeId)
        {
            Specification<SalaryCard> specification = new DirectSpecification<SalaryCard>(x => x.EmployeeId == employeeId);

            return specification;
        }

        public static ISpecification<SalaryCard> SalaryCardWithSalaryGroupId(Guid salaryGroupId)
        {
            Specification<SalaryCard> specification = new DirectSpecification<SalaryCard>(x => x.SalaryGroupId == salaryGroupId);

            return specification;
        }

        public static Specification<SalaryCard> SalaryCardFullText(string text)
        {
            Specification<SalaryCard> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<SalaryCard>(c => c.Employee.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<SalaryCard>(c => c.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));

                var addressEmail = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<SalaryCard>(c => c.Employee.Customer.Address.MobileLine.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var cardNumberSpec = new DirectSpecification<SalaryCard>(x => x.CardNumber == number);

                    specification |= cardNumberSpec;
                }
            }

            return specification;
        }
    }
}
