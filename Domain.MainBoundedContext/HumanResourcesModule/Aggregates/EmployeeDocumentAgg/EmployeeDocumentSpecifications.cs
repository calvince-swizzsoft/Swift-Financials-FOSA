using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDocumentAgg
{
    public static class EmployeeDocumentSpecifications
    {
        public static Specification<EmployeeDocument> DefaultSpec()
        {
            Specification<EmployeeDocument> specification = new TrueSpecification<EmployeeDocument>();

            return specification;
        }

        public static ISpecification<EmployeeDocument> EmployeeDocumentWithEmployeeId(Guid employeeId)
        {
            Specification<EmployeeDocument> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty)
            {
                specification &= new DirectSpecification<EmployeeDocument>(x => x.EmployeeId == employeeId);
            }

            return specification;
        }

        public static Specification<EmployeeDocument> EmployeeDocumentFullText(string text)
        {
            Specification<EmployeeDocument> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var membershipNumberSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.SerialNumber == number);

                    var payrollNumbersSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));

                    var addressLandLineSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.MobileLine.Contains(text));

                    var reference1Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Reference1.Contains(text));
                    var reference2Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Reference2.Contains(text));
                    var reference3Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Reference3.Contains(text));

                    specification &= (membershipNumberSpec | payrollNumbersSpec | identificationNumberSpec | addressLandLineSpec | addressMobileLineSpec | reference1Spec | reference2Spec | reference3Spec);
                }
                else
                {
                    var personalIdentificationNumberSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                    var nationalSocialSecurityFundNumberSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.NationalSocialSecurityFundNumber.Contains(text));
                    var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                    var firstNameSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Individual.FirstName.Contains(text));
                    var lastNameSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Individual.LastName.Contains(text));
                    var payrollNumbersSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));

                    var addressEmailSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.Email.Contains(text));
                    var addressCitySpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.City.Contains(text));
                    var addressPostalCodeSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.PostalCode.Contains(text));
                    var addressAddressLine1Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.AddressLine1.Contains(text));
                    var addressAddressLine2Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.AddressLine2.Contains(text));
                    var addressStreetSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.Street.Contains(text));
                    var addressLandLineSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Address.MobileLine.Contains(text));

                    var reference1Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Reference1.Contains(text));
                    var reference2Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Reference2.Contains(text));
                    var reference3Spec = new DirectSpecification<EmployeeDocument>(c => c.Employee.Customer.Reference3.Contains(text));

                    var fileTitleSpec = new DirectSpecification<EmployeeDocument>(c => c.FileTitle.Contains(text));
                    var fileDescriptionSpec = new DirectSpecification<EmployeeDocument>(c => c.FileDescription.Contains(text));
                    var fileMIMETypeSpec = new DirectSpecification<EmployeeDocument>(c => c.FileMIMEType.Contains(text));

                    specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                        | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec
                    | addressEmailSpec | addressCitySpec | addressPostalCodeSpec | addressAddressLine1Spec | addressAddressLine2Spec | addressStreetSpec | addressLandLineSpec | addressMobileLineSpec
                  | reference1Spec | reference2Spec | reference3Spec | fileTitleSpec | fileDescriptionSpec | fileMIMETypeSpec);
                }
            }

            return specification;
        }
    }
}
