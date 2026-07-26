using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditOfficerAgg
{
    public static class MicroCreditOfficerSpecifications
    {
        public static Specification<MicroCreditOfficer> DefaultSpec()
        {
            Specification<MicroCreditOfficer> specification = new TrueSpecification<MicroCreditOfficer>();

            return specification;
        }

        public static ISpecification<MicroCreditOfficer> MicroCreditOfficerWithEmployeeId(Guid employeeId)
        {
            Specification<MicroCreditOfficer> specification = new DirectSpecification<MicroCreditOfficer>(x => x.EmployeeId == employeeId);

            return specification;
        }

        public static Specification<MicroCreditOfficer> MicroCreditOfficerFullText(string text)
        {
            Specification<MicroCreditOfficer> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Reference3.Contains(text));

                var addressEmailSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.Email.Contains(text));
                var addressCitySpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.City.Contains(text));
                var addressPostalCodeSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1Spec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2Spec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.AddressLine2.Contains(text));
                var addressStreetSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Employee.Customer.Address.MobileLine.Contains(text));

                var remarksSpec = new DirectSpecification<MicroCreditOfficer>(c => c.Remarks.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                    | addressEmailSpec | addressCitySpec | addressPostalCodeSpec | addressAddressLine1Spec | addressAddressLine2Spec | addressStreetSpec | addressLandLineSpec | addressMobileLineSpec
                    | remarksSpec);
            }

            return specification;
        }
    }
}
