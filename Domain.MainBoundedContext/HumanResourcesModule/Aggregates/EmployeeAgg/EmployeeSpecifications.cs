using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg
{
    public static class EmployeeSpecifications
    {
        public static Specification<Employee> DefaultSpec()
        {
            Specification<Employee> specification = new TrueSpecification<Employee>();

            return specification;
        }

        public static ISpecification<Employee> EmployeeWithCustomerId(Guid customerId)
        {
            Specification<Employee> specification = new DirectSpecification<Employee>(x => x.CustomerId == customerId);

            return specification;
        }

        public static ISpecification<Employee> EmployeeWithCustomerSerialNumber(int serialNumber)
        {
            Specification<Employee> specification = new DirectSpecification<Employee>(x => x.Customer.SerialNumber == serialNumber);

            return specification;
        }

        public static ISpecification<Employee> EmployeeWithBranchId(Guid branchId)
        {
            Specification<Employee> specification = new DirectSpecification<Employee>(x => x.BranchId == branchId);

            return specification;
        }

        public static ISpecification<Employee> EmployeeWithDepartmentId(Guid departmentId)
        {
            Specification<Employee> specification = new DirectSpecification<Employee>(x => x.DepartmentId == departmentId);

            return specification;
        }

        public static Specification<Employee> EmployeeFullText(string text)
        {
            Specification<Employee> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<Employee>(c => c.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<Employee>(c => c.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<Employee>(c => c.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<Employee>(c => c.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<Employee>(c => c.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<Employee>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<Employee>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<Employee>(c => c.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<Employee>(c => c.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<Employee>(c => c.Customer.Reference3.Contains(text));

                var addressEmailSpec = new DirectSpecification<Employee>(c => c.Customer.Address.Email.Contains(text));
                var addressCitySpec = new DirectSpecification<Employee>(c => c.Customer.Address.City.Contains(text));
                var addressPostalCodeSpec = new DirectSpecification<Employee>(c => c.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1Spec = new DirectSpecification<Employee>(c => c.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2Spec = new DirectSpecification<Employee>(c => c.Customer.Address.AddressLine2.Contains(text));
                var addressStreetSpec = new DirectSpecification<Employee>(c => c.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<Employee>(c => c.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<Employee>(c => c.Customer.Address.MobileLine.Contains(text));

                var remarksSpec = new DirectSpecification<Employee>(c => c.Remarks.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                    | addressEmailSpec | addressCitySpec | addressPostalCodeSpec | addressAddressLine1Spec | addressAddressLine2Spec | addressStreetSpec | addressLandLineSpec | addressMobileLineSpec
                    | remarksSpec);
            }

            return specification;
        }

        public static ISpecification<Employee> EmployeesByDepartmentIdWithText(Guid departmentId, string text)
        {
            Specification<Employee> specification = new DirectSpecification<Employee>(x => x.DepartmentId == departmentId);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fullTextSpec = EmployeeFullText(text);

                specification &= fullTextSpec;
            }

            return specification;
        }
    }
}
