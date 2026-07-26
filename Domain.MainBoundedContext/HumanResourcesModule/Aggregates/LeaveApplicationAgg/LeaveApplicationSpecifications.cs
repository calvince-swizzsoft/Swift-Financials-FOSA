using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveApplicationAgg
{
    public static class LeaveApplicationSpecifications
    {
        public static Specification<LeaveApplication> DefaultSpec()
        {
            Specification<LeaveApplication> specification = new TrueSpecification<LeaveApplication>();

            return specification;
        }

        public static Specification<LeaveApplication> LeaveApplicationWithEmployeeId(Guid employeeId)
        {
            Specification<LeaveApplication> specification = new DirectSpecification<LeaveApplication>(x => x.EmployeeId == employeeId);

            return specification;
        }

        public static Specification<LeaveApplication> LeaveApplicationFullText(string text)
        {
            Specification<LeaveApplication> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));

                var addressEmail = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.MobileLine.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }

        public static Specification<LeaveApplication> LeaveApplicationsWithDateRangeAndStatus(DateTime startDate, DateTime endDate, int status, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LeaveApplication> specification = new DirectSpecification<LeaveApplication>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Individual.IdentityCardNumber.Contains(text));

                var addressEmail = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<LeaveApplication>(c => c.Employee.Customer.Address.MobileLine.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }

        public static Specification<LeaveApplication> ActiveLeaveApplicationWithEmployeeId(Guid employeeId)
        {
            var targetDate = DateTime.Today;

            Specification<LeaveApplication> specification = new DirectSpecification<LeaveApplication>(x =>
                x.EmployeeId == employeeId &&
                x.Status == (int)LeaveApplicationStatus.Approved &&
                x.Duration.StartDate <= targetDate && x.Duration.EndDate >= targetDate);

            return specification;
        }

        public static Specification<LeaveApplication> LeaveApplicationsByEmployeeId(Guid employeeId)
        {
            var targetDate = DateTime.Today;

            Specification<LeaveApplication> specification = new DirectSpecification<LeaveApplication>(x => x.EmployeeId == employeeId);

            return specification;
        }

        public static Specification<LeaveApplication> LeaveApplicationsWithEmployeeIdAndLeaveTypeId(Guid employeeId, Guid leaveTypeId)
        {
            var targetDate = DateTime.Today;

            Specification<LeaveApplication> specification = new DirectSpecification<LeaveApplication>(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId);

            return specification;
        }
    }
}
